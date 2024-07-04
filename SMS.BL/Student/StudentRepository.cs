/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using SMS.Data;
using SMS.Model.Student;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.BL.Student.Interface;
using SMS.ViewModel.Student;

namespace SMS.BL.Student
{
    public class StudentRepository : IStudentRepository
    {
        private readonly SMS_StoredContext _context;

        public StudentRepository(SMS_StoredContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all the student details
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public RepositoryResponse<IEnumerable<StudentBO>> GetStudents(bool? isEnable)
        {
            var response=new RepositoryResponse<IEnumerable<StudentBO>>();
            try
            {
                var statusParam = new SqlParameter("@IsEnable", isEnable.HasValue ? isEnable.Value : DBNull.Value);

                // Execute the stored procedure
                var students = _context.Students
                                       .FromSqlRaw("EXEC Get_Students @IsEnable", statusParam).ToList();

                response.Data=students;
                response.Success=true;
              
            }
            catch (Exception)
            {
                response.Success= false;
                response.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Students"));
                
            }
            return response;
        }

        /// <summary>
        /// Get student detials by ID
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        public RepositoryResponse<StudentBO> GetStudentByID(long studentId)
        {
            var response = new RepositoryResponse<StudentBO>();
            try
            {
                // Execute the stored procedure
                var student = _context.Students
                    .FromSqlRaw("EXEC Get_StudentById @StudentId = {0}", studentId)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (student != null)
                {
                    response.Data = student; 
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Messages.Add("Student not found.");
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                response.Success = false;
                response.Messages.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Add or edit student details
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> UpsertStudent(StudentBO student)
        {
            var response= new RepositoryResponse<bool>();

            try
            {
                var studentIDParam = new SqlParameter("@StudentID", (object)student.StudentID ?? DBNull.Value);
                var studentRegNoParam = new SqlParameter("@StudentRegNo", student.StudentRegNo);
                var firstNameParam = new SqlParameter("@FirstName", student.FirstName);
                var middleNameParam = new SqlParameter("@MiddleName", (object)student.MiddleName ?? DBNull.Value);
                var lastNameParam = new SqlParameter("@LastName", student.LastName);
                var displayNameParam = new SqlParameter("@DisplayName", student.DisplayName);
                var emailParam = new SqlParameter("@Email", student.Email);
                var genderParam = new SqlParameter("@Gender", student.Gender);
                var dobParam = new SqlParameter("@DOB", student.DOB);
                var addressParam = new SqlParameter("@Address", student.Address);
                var contactNoParam = new SqlParameter("@ContactNo", student.ContactNo);
                var isEnableParam = new SqlParameter("@IsEnable", student.IsEnable);
                var resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_AddOrEdit_Student @StudentID, @StudentRegNo, @FirstName, @MiddleName, @LastName, @DisplayName, @Email, @Gender, @DOB, @Address, @ContactNo, @IsEnable, @Result OUTPUT",
                                                 studentIDParam, studentRegNoParam, firstNameParam, middleNameParam, lastNameParam,
                                                 displayNameParam, emailParam, genderParam, dobParam, addressParam, contactNoParam, isEnableParam, resultParam);

                int result = (int)resultParam.Value;

                if (result > 0)
                {
                    response.Messages.Add(student.StudentID == null ? "Student added successfully!" : "Student details updated successfully");
                    response.Success=true;
                    return response;
                }
                else
                {
                    response.Messages.Add("Failed to save student details.");
                    response.Success=false;
                    return response;
                }
            }
            catch (SqlException ex)
            {
                response.Messages.Add($"SQL Error: {ex.Message}");
                response.Success = false;
                return response;


            }
            //catch (Exception ex)
            //{
            //    msg = $"Error: {ex.Message}";
            //    return false;
            //}
        }
        /// <summary>
        /// Check the student regsitration number is exsit or not
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DoesStudentRegNoExist(string regNo)
        {
            var response = new RepositoryResponse<bool>();

            try
            {
                var stRegNo = new SqlParameter("@RegNo", regNo);
                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckStudentRegNoExists(@RegNo)", stRegNo, result);

                response.Data = (bool)result.Value;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"Error: {ex.Message}");
            }

            return response;
        }

        /// <summary>
        /// check the existence of display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DoesStudentDisplayNameExist(string displayName)
        {
            var response = new RepositoryResponse<bool>();
            try 
            {
                var stDisplayName = new SqlParameter("@DisplayName", displayName);

                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckDisplayNameExists(@DisplayName)", stDisplayName, result);

                response.Data = (bool)result.Value;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"Error: {ex.Message}");
            }

            return response;
        }
         /// <summary>
         /// check existence of email
         /// </summary>
         /// <param name="email"></param>
         /// <returns></returns>
        public RepositoryResponse<bool> DoesStudentEmailExist(string email)
        {
            var response = new RepositoryResponse<bool>();
            try
            {
                var stEmail = new SqlParameter("@Email", email);

                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckEmailExists(@Email)", stEmail, result);


                response.Data = (bool)result.Value;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"Error: {ex.Message}");
            }

            return response;

        }
       /// <summary>
       /// Delete 
       /// </summary>
       /// <param name="studentId"></param>
       /// <returns></returns>
        public RepositoryResponse<bool> DeleteStudent(long studentId)
        {
            var response = new RepositoryResponse<bool>();
            var studentIdParam = new SqlParameter("@StudentID", studentId);
            var messageParam = new SqlParameter
            {
                ParameterName = "@Message",
                SqlDbType = SqlDbType.NVarChar,
                Size = 200,
                Direction = ParameterDirection.Output
            };
            var requiresConfirmationParam = new SqlParameter
            {
                ParameterName = "@RequiresConfirmation",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };

            try
            {
                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_DeleteStudent @StudentID, @Message OUTPUT, @RequiresConfirmation OUTPUT",
                         studentIdParam, messageParam, requiresConfirmationParam);

                var message= messageParam.Value != DBNull.Value ? (string)messageParam.Value : string.Empty;
                bool requiresConfirmation = requiresConfirmationParam.Value != DBNull.Value && (bool)requiresConfirmationParam.Value;

                if (!requiresConfirmation)
                {
                    response.Success = true;
                    response.Data = true;
                }
                else
                {
                    response.Success = false;
                    response.Data = false;
                    response.Messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                response.Messages.Add(ex.Message);
                
                response.Success = false;
                response.Data = false;
               
            }

            return response;
        }
        /// <summary>
        /// Toggle student status
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>   
        public RepositoryResponse<bool> ToggleStudentEnable(long studentId)
        {
            var response=new RepositoryResponse<bool>();
            var studentIdParam = new SqlParameter("@StudentID", studentId);
            var messageParam = new SqlParameter
            {
                ParameterName = "@Message",
                SqlDbType = SqlDbType.NVarChar,
                Size = 200,
                Direction = ParameterDirection.Output
            };
            var successParam = new SqlParameter
            {
                ParameterName = "@Success",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };
            try
            {
                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_ToggleStudentEnable @StudentID, @Message OUTPUT, @Success OUTPUT",
                                             studentIdParam, messageParam, successParam);

                var message = (string)messageParam.Value;
                response.Data=(bool)successParam.Value;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"Error: {ex.Message}");
            }

            return response;

        }
       /// <summary>
       /// Serach filter using model
       /// </summary>
       /// <param name="searchModel"></param>
       /// <returns></returns>
        public RepositoryResponse<IEnumerable<StudentBO>> SearchStudents(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<StudentBO>>();
            try
            {
                var searchTextParam = new SqlParameter("@SearchText", (object)searchModel.SearchText ?? DBNull.Value);
                var searchCategoryParam = new SqlParameter("@SearchCategory", (object)searchModel.SearchCategory ?? DBNull.Value);

                // Execute the stored procedure
                var students = _context.Students
                    .FromSqlRaw("EXEC dbo.usp_SearchStudents @SearchText, @SearchCategory", searchTextParam, searchCategoryParam)
                    .ToList();

                response.Data = students;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"Error: {ex.Message}");
            }

            return response;
        }

        /// <summary>
        /// Check whether the student is allocated for subject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> CheckStudentAllocationStatus(long id)
        {
            var response = new RepositoryResponse<bool>();
            var stdID = new SqlParameter("@StudentID", id);
            var result = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };

            try
            {
                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_CheckStudentAllocation @StudentID, @Result OUTPUT", stdID, result);

               
                response.Data = result.Value != DBNull.Value && (bool)result.Value;
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add($"Error: {ex.Message}");
            }

            return response;
        }

    }

}

