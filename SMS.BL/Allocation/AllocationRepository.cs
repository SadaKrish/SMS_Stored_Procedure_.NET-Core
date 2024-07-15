using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMS.BL.Allocation.Interface;
using SMS.Data;
using SMS.Model.Teacher_Subject_Allocation;
using SMS.ViewModel.Allocation;
using SMS.ViewModel.AllocationViewModel;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SMS.BL.Allocation
{
    public class AllocationRepository: IAllocationRepository
    {
        private readonly SMS_StoredContext _context;

        public AllocationRepository(SMS_StoredContext context)
        {
            _context = context;
        }
        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        public RepositoryResponse<IEnumerable<SubjectAllocationDetailViewModel>> GetAllSubjectAllocations()
        {
            var response = new RepositoryResponse<IEnumerable<SubjectAllocationDetailViewModel>>();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "GetAllSubjectAllocations";
                    command.CommandType = CommandType.StoredProcedure;

                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var data = new List<SubjectAllocationDetailViewModel>();

                        var columnNames = new List<string>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            columnNames.Add(reader.GetName(i));
                        }
                        Console.WriteLine("Columns: " + string.Join(", ", columnNames));

                        while (reader.Read())
                        {
                            try
                            {
                                var subjectCode = reader.GetString(reader.GetOrdinal("SubjectCode"));
                                var subjectName = reader.GetString(reader.GetOrdinal("SubjectName"));

                                if (!columnNames.Contains("Teachers"))
                                {
                                    throw new IndexOutOfRangeException("The 'Teachers' column is missing from the result set.");
                                }

                                string teachersString = reader.IsDBNull(reader.GetOrdinal("Teachers"))
                                    ? string.Empty
                                    : reader.GetString(reader.GetOrdinal("Teachers"));

                                var teachers = JsonConvert.DeserializeObject<List<TeacherAllocationViewModel>>(teachersString);

                                data.Add(new SubjectAllocationDetailViewModel
                                {
                                    SubjectCode = subjectCode,
                                    SubjectName = subjectName,
                                    Teachers = teachers
                                });
                            }
                            catch (IndexOutOfRangeException indexEx)
                            {
                                Console.WriteLine($"IndexOutOfRangeException: {indexEx.Message}");
                            }
                            catch (InvalidCastException castEx)
                            {
                                Console.WriteLine($"InvalidCastException: {castEx.Message}");
                            }
                        }

                        response.Data = data;
                        response.Success = true;
                    }

                    _context.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subject Allocations"));
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEnabledTeachersList()
        {
            try
            {
                var teachers = _context.Teachers
                    .FromSqlRaw("EXEC usp_GetEnabledTeachers")
                    .ToList(); 
               
                //if (teachers == null || !teachers.Any())
                //{
                //    Console.WriteLine("No enabled teachers found.");
                //    return Enumerable.Empty<SelectListItem>();
                //}

                var selectList = teachers.Select(t => new SelectListItem
                {
                    Value = t.TeacherID.ToString(),
                    Text = $"{t.TeacherRegNo} - {t.DisplayName}"
                }).ToList();
                //if (selectList == null || !selectList.Any())
                //{
                //    Console.WriteLine("No select list items created.");
                //    return Enumerable.Empty<SelectListItem>();
                //}

                return selectList;
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Enumerable.Empty<SelectListItem>();
            }
        }



        public IEnumerable<SelectListItem> GetEnabledSubjectList()
        {
            try
            {
                var subjects = _context.Subjects
                    .FromSqlRaw("EXEC usp_GetEnabledSubjects")
                    .ToList(); 

                if (subjects == null || !subjects.Any())
                {
                    Console.WriteLine("No enabled teachers found.");
                    return Enumerable.Empty<SelectListItem>();
                }

                var selectList = subjects.Select(t => new SelectListItem
                {
                    Value = t.SubjectID.ToString(),
                    Text = $"{t.SubjectCode} - {t.Name}"
                }).ToList(); 
                if (selectList == null || !selectList.Any())
                {
                    Console.WriteLine("No select list items created.");
                    return Enumerable.Empty<SelectListItem>();
                }

                return selectList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Enumerable.Empty<SelectListItem>();
            }
        }



        public Teacher_Subject_AllocationBO GetSubjectAllocationById(long subjectAllocationId)
        {
            var subjectAllocationIdParam = new SqlParameter("@SubjectAllocationID", subjectAllocationId);

            var result = _context.Teacher_Subject_Allocations
                .FromSqlRaw("EXEC usp_GetSubjectAllocationById @SubjectAllocationID", subjectAllocationIdParam)
                .AsEnumerable()  
                .Select(s => new Teacher_Subject_AllocationBO
                {
                    SubjectAllocationID = s.SubjectAllocationID,
                    TeacherID = s.TeacherID,
                    SubjectID = s.SubjectID
                })
                .FirstOrDefault();

            return result;
        }

        public RepositoryResponse<bool> UpsertSubjectAllocation(Teacher_Subject_AllocationBO allocation)
        {
            var response = new RepositoryResponse<bool>();

            try
            {
               
                var teacherIdParam = new SqlParameter("@TeacherID", allocation.TeacherID);
                var subjectIdParam = new SqlParameter("@SubjectID", allocation.SubjectID);
                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

              
                _context.Database.ExecuteSqlRaw("EXEC usp_UpsertSubjectAllocation @TeacherID, @SubjectID, @Message OUTPUT, @Success OUTPUT",
                    teacherIdParam, subjectIdParam, messageParam, successParam);

               
                response.Messages.Add(messageParam.Value.ToString());
                response.Success = (bool)successParam.Value;
                response.Data = response.Success; 
            }
            catch (Exception ex)
            {
                
                response.Success = false;
                response.Messages.Add(ex.InnerException != null
                    ? "An error occurred while processing the request: " + ex.InnerException.Message
                    : "An error occurred while processing the request: " + ex.Message);
            }

            return response;
        }
        /// <summary>
        /// Delete the subject allocation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        public RepositoryResponse<bool> DeleteSubjectAllocation(long id)
        {
            var response = new RepositoryResponse<bool>();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_DeleteSubjectAllocation";
                    command.CommandType = CommandType.StoredProcedure;

                    var idParam = command.CreateParameter();
                    idParam.ParameterName = "@SubjectAllocationID";
                    idParam.DbType = DbType.Int64;
                    idParam.Value = id;
                    command.Parameters.Add(idParam);

                    var resultMessageParam = command.CreateParameter();
                    resultMessageParam.ParameterName = "@ResultMessage";
                    resultMessageParam.DbType = DbType.String;
                    resultMessageParam.Size = 4000; 
                    resultMessageParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultMessageParam);

                    _context.Database.OpenConnection();

                  
                    command.ExecuteNonQuery();

                    var resultMessage = (string)resultMessageParam.Value;

                    if (resultMessage.Contains("successfully"))
                    {
                        response.Data = true;
                        response.Success = true;
                        response.Messages.Add(resultMessage);
                    }
                    else
                    {
                        response.Data = false;
                        response.Success = false;
                        response.Messages.Add(resultMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.Success = false;
                response.Messages.Add(string.Format(StaticMessages.Error_Delete_Data, "Subject Allocation"));
                Console.WriteLine(ex.Message);
            }

            return response;
        }


        public RepositoryResponse<IEnumerable<Teacher_Subject_AllocationBO>> SearchSubjectAllocations(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<Teacher_Subject_AllocationBO>>();
            try
            {
                // Ensure the search text and category are not null
                var searchText = (object)searchModel.SearchText ?? DBNull.Value;
                var searchCategory = (object)searchModel.SearchCategory ?? DBNull.Value;

                var searchTextParam = new SqlParameter("@SearchText", searchText);
                var searchCategoryParam = new SqlParameter("@SearchCategory", searchCategory);

                // Execute the stored procedure
                var allocations = _context.Teacher_Subject_Allocations
                    .FromSqlRaw("EXEC dbo.usp_SearchSubjectAllocations @SearchText, @SearchCategory", searchTextParam, searchCategoryParam)
                    .ToList();

                response.Data = allocations;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"Error: {ex.Message}");
            }

            return response;
        }

        public RepositoryResponse<IEnumerable<Teacher_Subject_AllocationBO>> GetSubjectAllocationByTerm(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<Teacher_Subject_AllocationBO>>();
            try
            {
                var parameters = new[]
                {
                     new SqlParameter("@term", (object)searchModel.SearchText ?? DBNull.Value),
                     new SqlParameter("@category", (object)searchModel.SearchCategory ?? DBNull.Value)
                };
                var results = _context.Teacher_Subject_Allocations.FromSqlRaw("EXEC usp_GetTeacherSearchSuggestions @term, @category", parameters)
                                               .ToList();
                response.Data = results;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"{ex.Message}");
            }
            return response;
        }

        public RepositoryResponse<IEnumerable<StudentAllocationGroupedViewModel>> GetAllStudentAllocations(bool? status = null)
        {
            var response = new RepositoryResponse<IEnumerable<StudentAllocationGroupedViewModel>>();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_GetAllStudentAllocations";
                    command.CommandType = CommandType.StoredProcedure;

                    var statusParam = new SqlParameter("@status", status.HasValue ? (object)status.Value : DBNull.Value);
                    command.Parameters.Add(statusParam);

                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var studentAllocations = new List<StudentAllocationGroupedViewModel>();

                        while (reader.Read())
                        {
                            var studentRegNo = reader.IsDBNull(reader.GetOrdinal("StudentRegNo")) ? null : reader.GetString(reader.GetOrdinal("StudentRegNo"));
                            var displayName = reader.IsDBNull(reader.GetOrdinal("DisplayName")) ? null : reader.GetString(reader.GetOrdinal("DisplayName"));
                            var isEnable = reader.IsDBNull(reader.GetOrdinal("IsEnable")) ? false : reader.GetBoolean(reader.GetOrdinal("IsEnable"));
                            var subjectsString = reader.IsDBNull(reader.GetOrdinal("Subjects")) ? string.Empty : reader.GetString(reader.GetOrdinal("Subjects"));

                            var student = studentAllocations.FirstOrDefault(s => s.StudentRegNo == studentRegNo);
                            if (student == null)
                            {
                                student = new StudentAllocationGroupedViewModel
                                {
                                    StudentRegNo = studentRegNo,
                                    DisplayName = displayName,
                                    IsEnable = isEnable,
                                    Subjects = new List<SubjectAllocationViewModel>() // Initialize Subjects list
                                };
                                studentAllocations.Add(student);
                            }

                            // Parse the combined subjects and teachers string
                            ParseSubjects(subjectsString, student);
                        }

                        response.Data = studentAllocations;
                        response.Success = true;
                    }

                    _context.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Student Allocations"));
                Console.WriteLine(ex.Message);
            }

            return response;
        }

        private void ParseSubjects(string subjectsString, StudentAllocationGroupedViewModel student)
        {
            if (string.IsNullOrEmpty(subjectsString))
                return;

            // Split the subjects by comma and space
            var subjectsArray = subjectsString.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var subjectDetail in subjectsArray)
            {
                // Split the subject detail by ':'
                var subjectParts = subjectDetail.Split(": ", 2);
                if (subjectParts.Length < 2)
                    continue;

                // Get the subject code and the JSON string of teacher allocations
                var subjectCode = subjectParts[0];
                var teachersJson = subjectParts[1];

                // Create or get the subject view model
                var subjectViewModel = student.Subjects.FirstOrDefault(s => s.SubjectCode == subjectCode);
                if (subjectViewModel == null)
                {
                    subjectViewModel = new SubjectAllocationViewModel
                    {
                        SubjectCode = subjectCode,
                        SubjectName = subjectCode, // Adjust this if you have a specific way to determine SubjectName
                        TeacherAllocations = new List<TeacherAllocationViewModel>()
                    };
                    student.Subjects.Add(subjectViewModel);
                }

                // Parse teacher allocations from JSON
                try
                {
                    var teacherAllocations = Newtonsoft.Json.JsonConvert.DeserializeObject<List<TeacherAllocationViewModel>>(teachersJson);
                    if (teacherAllocations != null)
                    {
                        foreach (var teacher in teacherAllocations)
                        {
                            // Add teacher allocation to the subject's list
                            if (!subjectViewModel.TeacherAllocations.Any(t => t.TeacherRegNo == teacher.TeacherRegNo))
                            {
                                subjectViewModel.TeacherAllocations.Add(teacher);
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    // Handle JSON deserialization errors
                    Console.WriteLine($"Error parsing teacher allocations for subject {subjectCode}: {ex.Message}");
                }
            }
        }




        public RepositoryResponse<bool> DeleteStudentAllocation(long id)
        {
            var response = new RepositoryResponse<bool>();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_DeleteStudentAllocation";
                    command.CommandType = CommandType.StoredProcedure;

                    var idParam = command.CreateParameter();
                    idParam.ParameterName = "@StudentAllocationID";
                    idParam.DbType = DbType.Int64;
                    idParam.Value = id;
                    command.Parameters.Add(idParam);

                    var resultMessageParam = command.CreateParameter();
                    resultMessageParam.ParameterName = "@ResultMessage";
                    resultMessageParam.DbType = DbType.String;
                    resultMessageParam.Size = 4000;
                    resultMessageParam.Direction = ParameterDirection.Output;
                    command.Parameters.Add(resultMessageParam);

                    _context.Database.OpenConnection();


                    command.ExecuteNonQuery();

                    var resultMessage = (string)resultMessageParam.Value;

                    if (resultMessage.Contains("successfully"))
                    {
                        response.Data = true;
                        response.Success = true;
                        response.Messages.Add(resultMessage);
                    }
                    else
                    {
                        response.Data = false;
                        response.Success = false;
                        response.Messages.Add(resultMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                response.Data = false;
                response.Success = false;
                response.Messages.Add(string.Format(StaticMessages.Error_Delete_Data, "Student Allocation"));
                Console.WriteLine(ex.Message);
            }

            return response;
        }








    }
}
