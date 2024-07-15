/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.BL.Teacher.Interface;
using SMS.Data;
using SMS.Model.Student;
using SMS.Model.Teacher;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.ViewModel.Teacher;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Teacher
{
    public class TeacherRepository: ITeacherRepository
    {
        private readonly SMS_StoredContext _context;

        public TeacherRepository(SMS_StoredContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all teachers
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public RepositoryResponse<IEnumerable<TeacherBO>> GetTeachers(bool? isEnable)
        {
            var response = new RepositoryResponse<IEnumerable<TeacherBO>>();
            try
            {
                var statusParam = new SqlParameter("@IsENable", isEnable.HasValue ? isEnable.Value : DBNull.Value);
                var teachers = _context.Teachers.FromSqlRaw("EXEC usp_Get_Teachers @IsEnable", statusParam).ToList();

                response.Data = teachers;
                response.Success = true;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teachers"));
            }
            return response;
        }
        /// <summary>
        /// Get teacher by ID
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        public RepositoryResponse<TeacherBO> GetTeacherByID(long teacherId)
        {
            var response = new RepositoryResponse<TeacherBO>();
            try
            {
                // Execute the stored procedure
                var teacher = _context.Teachers
                    .FromSqlRaw("EXEC usp_Get_TeacherByID @TeacherId = {0}", teacherId)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (teacher != null)
                {
                    response.Data = teacher;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Messages.Add("Teacher not found.");
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Messages.Add(ex.Message);
            }

            return response;
        }
        /// <summary>
        /// Add or update the teacher details
        /// </summary>
        /// <param name="teacher"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> UpsertTeacher(TeacherBO teacher)
        {
            var response = new RepositoryResponse<bool>();

            try
            {
                var teacherIDParam = new SqlParameter("@TeacherID", (object)teacher.TeacherID ?? DBNull.Value);
                var teacherRegNoParam = new SqlParameter("@TeacherRegNo", teacher.TeacherRegNo);
                var firstNameParam = new SqlParameter("@FirstName", teacher.FirstName);
                var middleNameParam = new SqlParameter("@MiddleName", (object)teacher.MiddleName ?? DBNull.Value);
                var lastNameParam = new SqlParameter("@LastName", teacher.LastName);
                var displayNameParam = new SqlParameter("@DisplayName", teacher.DisplayName);
                var emailParam = new SqlParameter("@Email", teacher.Email);
                var genderParam = new SqlParameter("@Gender", teacher.Gender);
                var dobParam = new SqlParameter("@DOB", teacher.DOB);
                var addressParam = new SqlParameter("@Address", teacher.Address);
                var contactNoParam = new SqlParameter("@ContactNo", teacher.ContactNo);
                var isEnableParam = new SqlParameter("@IsEnable", teacher.IsEnable);
                var resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_Upsert_Teacher @TeacherID, @TeacherRegNo, @FirstName, @MiddleName, @LastName, @DisplayName, @Email, @Gender, @DOB, @Address, @ContactNo, @IsEnable, @Result OUTPUT",
                                                 teacherIDParam, teacherRegNoParam, firstNameParam, middleNameParam, lastNameParam,
                                                 displayNameParam, emailParam, genderParam, dobParam, addressParam, contactNoParam, isEnableParam, resultParam);

                int result = (int)resultParam.Value;

                if (result > 0)
                {
                    response.Messages.Add(teacher.TeacherID == null ? "Teacher added successfully!" : "Teacher details updated successfully");
                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Messages.Add("Failed to save teacher details.");
                    response.Success = false;
                    return response;
                }
            }
            catch (SqlException ex)
            {
                response.Messages.Add($"SQL Error: {ex.Message}");
                response.Success = false;
                return response;


            }
        }
        /// <summary>
        /// Check the existence of Teacher reg no
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DoesTeacherRegNoExist(string regNo)
        {
            var response = new RepositoryResponse<bool>();

            try
            {
                var teRegNo = new SqlParameter("@RegNo", regNo);
                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckTeacherRegNoExists(@RegNo)", teRegNo, result);

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
        /// Check the existence of Display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DoesTeacherDisplayNameExist(string displayName)
        {
            var response = new RepositoryResponse<bool>();
            try
            {
                var teDisplayName = new SqlParameter("@DisplayName", displayName);

                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckTeacherDisplayNameExists(@DisplayName)", teDisplayName, result);

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
        public RepositoryResponse<bool> DoesTeacherEmailExist(string email)
        {
            var response = new RepositoryResponse<bool>();
            try
            {
                var teEmail = new SqlParameter("@Email", email);

                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckTeacherEmailExists(@Email)", teEmail, result);


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
        /// <param name="teacherId"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DeleteTeacher(long teacherId)
        {
            var response = new RepositoryResponse<bool>();
            var teacherIdParam = new SqlParameter("@TeacherID", teacherId);
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
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_DeleteTeacher @TeacherID, @Message OUTPUT, @RequiresConfirmation OUTPUT",
                         teacherIdParam, messageParam, requiresConfirmationParam);

                var message = messageParam.Value != DBNull.Value ? (string)messageParam.Value : string.Empty;
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
        /// Toggle teacher
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>   
        public RepositoryResponse<bool> ToggleTeacherEnable(long teacherId)
        {
            var response = new RepositoryResponse<bool>();
            var teacherIdParam = new SqlParameter("@TeacherID", teacherId);
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
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_ToggleEnbaleTeacher @TeacherID, @Message OUTPUT, @Success OUTPUT",
                                             teacherIdParam, messageParam, successParam);

                var message = (string)messageParam.Value;
                var success = (bool)successParam.Value;
                if (success)
                {
                    response.Data = true;
                    response.Success = true;
                }
                else
                {
                    response.Data = false;
                    response.Success = false;
                }
                response.Messages.Add(message);
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
        public RepositoryResponse<IEnumerable<TeacherBO>> SearchTeachers(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<TeacherBO>>();
            try
            {
                var searchTextParam = new SqlParameter("@SearchText", (object)searchModel.SearchText ?? DBNull.Value);
                var searchCategoryParam = new SqlParameter("@SearchCategory", (object)searchModel.SearchCategory ?? DBNull.Value);

                // Execute the stored procedure
                var teachers = _context.Teachers
                    .FromSqlRaw("EXEC dbo.usp_SearchTeachers @SearchText, @SearchCategory", searchTextParam, searchCategoryParam)
                    .ToList();

                response.Data = teachers;
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
        /// 
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public RepositoryResponse<IEnumerable<TeacherBO>> GetTeachersByTerm(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<TeacherBO>>();
            try
            {
                var parameters = new[]
                {
                     new SqlParameter("@term", (object)searchModel.SearchText ?? DBNull.Value),
                     new SqlParameter("@category", (object)searchModel.SearchCategory ?? DBNull.Value)
                };
                var results = _context.Teachers.FromSqlRaw("EXEC usp_GetTeacherSearchSuggestions @term, @category", parameters)
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
        /// <summary>
        /// Check allocation status of teacher
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> CheckTeacherAllocationStatus(long id)
        {
            var response = new RepositoryResponse<bool>();
            var teacherID = new SqlParameter("@TeacherID", id);
            var result = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };

            try
            {
                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_CheckTeacherAllocation @TeacherID, @Result OUTPUT", teacherID, result);


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
