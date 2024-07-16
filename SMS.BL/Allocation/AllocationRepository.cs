using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SMS.BL.Allocation.Interface;
using SMS.Data;
using SMS.Model.Student_Subject_Teacher_Allocation;
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
        /// Get all subject allocations
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
        /// Get enabled teacher list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEnabledTeachersList()
        {
            try
            {
                var teachers = _context.Teachers
                    .FromSqlRaw("EXEC usp_GetEnabledTeachers")
                    .ToList(); 

                var selectList = teachers.Select(t => new SelectListItem
                {
                    Value = t.TeacherID.ToString(),
                    Text = $"{t.TeacherRegNo} - {t.DisplayName}"
                }).ToList();

                return selectList;
            }
            catch (Exception ex)
            {
              
                Console.WriteLine($"An error occurred: {ex.Message}");
                return Enumerable.Empty<SelectListItem>();
            }
        }
        /// <summary>
        /// Get Enbled subject list
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// Get subject allocation by its id
        /// </summary>
        /// <param name="subjectAllocationId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Add or edit subject allocation
        /// </summary>
        /// <param name="allocation"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Search options in subject allocation
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>

        public RepositoryResponse<IEnumerable<SubjectAllocationDetailViewModel>> SearchSubjectAllocations(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<SubjectAllocationDetailViewModel>>();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_SearchSubjectAllocations";
                    command.CommandType = CommandType.StoredProcedure;

                    var searchTextParam = new SqlParameter("@SearchText", searchModel.SearchText ?? (object)DBNull.Value);
                    var searchCategoryParam = new SqlParameter("@SearchCategory", searchModel.SearchCategory ?? (object)DBNull.Value);

                    command.Parameters.Add(searchTextParam);
                    command.Parameters.Add(searchCategoryParam);

                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        var data = new List<SubjectAllocationDetailViewModel>();

                        while (reader.Read())
                        {
                            try
                            {
                                var subjectCode = reader.IsDBNull(reader.GetOrdinal("SubjectCode"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("SubjectCode"));
                                var subjectName = reader.IsDBNull(reader.GetOrdinal("SubjectName"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("SubjectName"));

                                var teachersJson = reader.IsDBNull(reader.GetOrdinal("Teachers"))
                                    ? string.Empty
                                    : reader.GetString(reader.GetOrdinal("Teachers"));

                                var teachers = JsonConvert.DeserializeObject<List<TeacherAllocationViewModel>>(teachersJson);

                                data.Add(new SubjectAllocationDetailViewModel
                                {
                                    SubjectCode = subjectCode,
                                    SubjectName = subjectName,
                                    Teachers = teachers
                                });
                            }
                            catch (JsonException jsonEx)
                            {
                                Console.WriteLine($"Error parsing JSON for subject: {reader.GetString(reader.GetOrdinal("SubjectCode"))}. Exception: {jsonEx.Message}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Unexpected error: {ex.Message}");
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
                response.Messages.Add($"Error loading subject allocations: {ex.Message}");
                Console.WriteLine(ex.Message);
            }

            return response;
        }
        /************************************************************************Student Allocation**************************************************************/
        /// <summary>
        /// Get All student allocation
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
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
                            Console.WriteLine($"StudentRegNo: {studentRegNo}, DisplayName: {displayName}, SubjectsString: {subjectsString}");

                            var student = studentAllocations.FirstOrDefault(s => s.StudentRegNo == studentRegNo);
                            if (student == null)
                            {
                                student = new StudentAllocationGroupedViewModel
                                {
                                    StudentRegNo = studentRegNo,
                                    DisplayName = displayName,
                                    IsEnable = isEnable,
                                    Subjects = new List<SubjectAllocationViewModel>() 
                                };
                                studentAllocations.Add(student);
                            }

                          
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
        /// <summary>
        /// Parse the json output
        /// </summary>
        /// <param name="subjectsString"></param>
        /// <param name="student"></param>
        private void ParseSubjects(string subjectsString, StudentAllocationGroupedViewModel student)
        {
            if (string.IsNullOrEmpty(subjectsString))
                return;

            var subjectsArray = subjectsString.Split(", ", StringSplitOptions.RemoveEmptyEntries);

            foreach (var subjectDetail in subjectsArray)
            {
                var subjectParts = subjectDetail.Split(": ", 2);
                if (subjectParts.Length < 2)
                    continue;

                var subjectCode = subjectParts[0];
                var teachersJson = subjectParts[1];

                var subjectViewModel = student.Subjects.FirstOrDefault(s => s.SubjectCode == subjectCode);
                if (subjectViewModel == null)
                {
                    subjectViewModel = new SubjectAllocationViewModel
                    {
                        SubjectCode = subjectCode,
                        SubjectName = subjectCode, 
                        TeacherAllocations = new List<TeacherAllocationViewModel>()
                    };
                    student.Subjects.Add(subjectViewModel);
                }

                try
                {
                    var teacherAllocations = JsonConvert.DeserializeObject<List<TeacherAllocationViewModel>>(teachersJson);
                    if (teacherAllocations != null)
                    {
                        foreach (var teacher in teacherAllocations)
                        {
                            
                            Console.WriteLine($"Parsed Teacher Allocation ID: {teacher.StudentAllocationID}");

                            if (!subjectViewModel.TeacherAllocations.Any(t => t.TeacherRegNo == teacher.TeacherRegNo))
                            {
                                subjectViewModel.TeacherAllocations.Add(teacher);
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Error parsing teacher allocations for subject {subjectCode}: {ex.Message}");
                }
            }
        }
        /// <summary>
        /// Get enabled STudent list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEnabledStudentList()
        {
            try
            {
                var students = _context.Students
                    .FromSqlRaw("EXEC usp_GetEnabledStudents")
                    .ToList();

                if (students == null || !students.Any())
                {
                    Console.WriteLine("No enabled teachers found.");
                    return Enumerable.Empty<SelectListItem>();
                }

                var selectList = students.Select(t => new SelectListItem
                {
                    Value = t.StudentID.ToString(),
                    Text = $"{t.StudentRegNo} - {t.DisplayName}"
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
        /// <summary>
        /// get the teachers list from the allocation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetTeacherListFromAllocation()
        {
            var teacherList = new List<SelectListItem>();

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_GetTeacherListFromAllocation"; // Stored Procedure Name
                    command.CommandType = CommandType.StoredProcedure;

                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            teacherList.Add(new SelectListItem
                            {
                                Value = reader["TeacherID"].ToString(),
                                Text = reader["TeacherRegNo"] + " - " + reader["DisplayName"]
                            });
                        }
                    }

                    _context.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return teacherList;
        }
        /// <summary>
        /// Get all subjects list from allocation
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetAllSubjectsFromAllocation()
        {
            var subjectList = new List<SelectListItem>();

            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_GetSubjectListFromAllocation"; 
                    command.CommandType = CommandType.StoredProcedure;

                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            subjectList.Add(new SelectListItem
                            {
                                Value = reader["SubjectID"].ToString(),
                                Text = reader["SubjectCode"] + " - " + reader["Name"]
                            });
                        }
                    }

                    _context.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return subjectList;
        }
        /// <summary>
        /// Add student allocation
        /// </summary>
        /// <param name="allocation"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> AddStudentAllocation(Student_Subject_Teacher_AllocationBO allocation)
        {
            var response = new RepositoryResponse<bool>();

            try
            {
              
                var studentIdParam = new SqlParameter("@StudentID", allocation.StudentID);
                var subjectAllocationIdParam = new SqlParameter("@SubjectAllocationID", allocation.SubjectAllocationID);

               
                var studentAllocationIdParam = new SqlParameter("@StudentAllocationID",
                    allocation.StudentAllocationID.HasValue ? (object)allocation.StudentAllocationID.Value : DBNull.Value);

                var messageParam = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
                {
                    Direction = ParameterDirection.Output
                };
                var successParam = new SqlParameter("@Success", SqlDbType.Bit)
                {
                    Direction = ParameterDirection.Output
                };

               
                _context.Database.ExecuteSqlRaw(
                    "EXEC usp_UpsertStudentAllocation @StudentID, @SubjectAllocationID, @StudentAllocationID, @Message OUTPUT, @Success OUTPUT",
                    studentIdParam, subjectAllocationIdParam, studentAllocationIdParam, messageParam, successParam);

               
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
        /// Get teacher list accroding to the selected subject id
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        public IEnumerable<object> GetTeachersBySubjectID(long subjectID)
        {
            var result = new List<object>();

            try
            {
                // Create a new command to execute the stored procedure
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_GetTeachersBySubjectID"; // Name of the stored procedure
                    command.CommandType = CommandType.StoredProcedure;

                    // Add the parameter for the stored procedure
                    command.Parameters.Add(new SqlParameter("@SubjectID", SqlDbType.Int) { Value = subjectID });

                    // Open the connection
                    _context.Database.OpenConnection();

                    using (var reader = command.ExecuteReader())
                    {
                        // Read the data from the result set
                        while (reader.Read())
                        {
                            result.Add(new
                            {
                                teacherRegNo = reader["TeacherRegNo"].ToString(),
                                displayName = reader["DisplayName"].ToString(),
                                subjectAllocationID = Convert.ToInt64(reader["SubjectAllocationID"])
                            });
                        }
                    }

                    _context.Database.CloseConnection();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving teachers by subject ID: {ex.Message}");
            }
            finally
            {
                if (_context.Database.GetDbConnection().State == ConnectionState.Open)
                {
                    _context.Database.GetDbConnection().Close();
                }
            }

            return result;
        }
        /// <summary>
        /// Delete student allocation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
        /// <summary>
        /// Search option in student allocation
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public RepositoryResponse<IEnumerable<StudentAllocationGroupedViewModel>> SearchStudentAllocations(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<StudentAllocationGroupedViewModel>>();
            try
            {
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "usp_SearchStudentAllocations";
                    command.CommandType = CommandType.StoredProcedure;

                    // Add parameters for search text and category
                    var searchTextParam = new SqlParameter("@SearchText", searchModel.SearchText ?? (object)DBNull.Value);
                    var searchCategoryParam = new SqlParameter("@SearchCategory", searchModel.SearchCategory ?? (object)DBNull.Value);
                    command.Parameters.Add(searchTextParam);
                    command.Parameters.Add(searchCategoryParam);

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
                                    Subjects = new List<SubjectAllocationViewModel>()
                                };
                                studentAllocations.Add(student);
                            }

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

    }
}
