/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.BL.Subject.Interface;
using SMS.Data;
using SMS.Model.Subject;
using SMS.Model.Teacher;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.ViewModel.Subject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Subject
{
    public class SubjectRepository:ISubjectRepository
    {
        private readonly SMS_StoredContext _context;

        public SubjectRepository(SMS_StoredContext context)
        {
            _context = context;
        }
        /// <summary>
        /// Get all subjects
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        public RepositoryResponse<IEnumerable<SubjectBO>> GetSubjects(bool? isEnable)
        {
            var response = new RepositoryResponse<IEnumerable<SubjectBO>>();
            try
            {
                var statusParam = new SqlParameter("@IsEnable", isEnable.HasValue ? isEnable.Value : DBNull.Value);

                // Execute the stored procedure
                var subjects = _context.Subjects
                                       .FromSqlRaw("EXEC usp_Get_Subjects @IsEnable", statusParam).ToList();

                response.Data = subjects;
                response.Success = true;

            }
            catch (Exception)
            {
                response.Success = false;
                response.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subjects"));

            }
            return response;
        }
        /// <summary>
        /// Get subject by its id
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public RepositoryResponse<SubjectBO> GetSubjectByID(long subjectId)
        {
            var response = new RepositoryResponse<SubjectBO>();
            try
            {
                var subject = _context.Subjects
                    .FromSqlRaw("EXEC usp_Get_SubjectByID @SubjectId = {0}", subjectId)
                    .AsEnumerable()
                    .FirstOrDefault();

                if (subject != null)
                {
                    response.Data = subject;
                    response.Success = true;
                }
                else
                {
                    response.Success = false;
                    response.Messages.Add("Subject not found.");
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
        /// Update or add subject
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> UpsertSubject(SubjectBO subject)
        {
            var response = new RepositoryResponse<bool>();

            try
            {
                var subjectIDParam = new SqlParameter("@SubjectID", (object)subject.SubjectID ?? DBNull.Value);
                var subjectCodeParam = new SqlParameter("@SubjectCode", subject.SubjectCode);
                var nameParam = new SqlParameter("@Name", subject.Name);
                var isEnableParam = new SqlParameter("@IsEnable", subject.IsEnable);
                var resultParam = new SqlParameter("@Result", SqlDbType.Int) { Direction = ParameterDirection.Output };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_Upsert_Subject @SubjectID, @SubjectCode, @Name, @IsEnable, @Result OUTPUT",
                                                 subjectIDParam, subjectCodeParam, nameParam, isEnableParam, resultParam);

                int result = (int)resultParam.Value;

                if (result > 0)
                {
                    response.Messages.Add(subject.SubjectID == null ? "Subject added successfully!" : "Subject details updated successfully");
                    response.Success = true;
                    return response;
                }
                else
                {
                    response.Messages.Add("Failed to save subject details.");
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
            catch (Exception ex)
            {
                response.Messages.Add($"Error: {ex.Message}");
                response.Success = false;
                return response;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subCode"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DoesSubjectCodeExist(string subCode)
        {
            var response = new RepositoryResponse<bool>();

            try
            {
                var subjectCode = new SqlParameter("@SubCode", subCode);
                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckSubjectCodeExists(@SubCode)", subjectCode, result);

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
        /// <param name="subName"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DoesSubjectNameExist(string subName)
        {
            var response = new RepositoryResponse<bool>();
            try
            {
                var subjectName = new SqlParameter("@Name", subName);

                var result = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = SqlDbType.Bit,
                    Direction = ParameterDirection.Output
                };

                // Execute the stored procedure
                _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckSubjectNameExists(@Name)", subjectName, result);

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
        /// <param name="subjectId"></param>
        /// <returns></returns>
        public RepositoryResponse<bool> DeleteSubject(long subjectId)
        {
            var response = new RepositoryResponse<bool>();
            var subjectIdParam = new SqlParameter("@SubjectID", subjectId);
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
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_DeleteSubject @SubjectID, @Message OUTPUT, @RequiresConfirmation OUTPUT",
                         subjectIdParam, messageParam, requiresConfirmationParam);

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
        /// <param name="subjectId"></param>
        /// <returns></returns>   
        public RepositoryResponse<bool> ToggleSubjectEnable(long subjectId)
        {
            var response = new RepositoryResponse<bool>();
            var subjectIdParam = new SqlParameter("@SubjectID", subjectId);
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
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_ToggleEnableSubject @SubjectID, @Message OUTPUT, @Success OUTPUT",
                                             subjectIdParam, messageParam, successParam);

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
        public RepositoryResponse<IEnumerable<SubjectBO>> SearchSubjects(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<SubjectBO>>();
            try
            {
                var searchTextParam = new SqlParameter("@SearchText", (object)searchModel.SearchText ?? DBNull.Value);
                var searchCategoryParam = new SqlParameter("@SearchCategory", (object)searchModel.SearchCategory ?? DBNull.Value);

                // Execute the stored procedure
                var subjects = _context.Subjects
                    .FromSqlRaw("EXEC dbo.usp_SearchSubjects @SearchText, @SearchCategory", searchTextParam, searchCategoryParam)
                    .ToList();

                response.Data = subjects;
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
        public RepositoryResponse<IEnumerable<SubjectBO>> GetSubjectsByTerm(SearchViewModel searchModel)
        {
            var response = new RepositoryResponse<IEnumerable<SubjectBO>>();
            try
            {
                var parameters = new[]
                {
                     new SqlParameter("@term", (object)searchModel.SearchText ?? DBNull.Value),
                     new SqlParameter("@category", (object)searchModel.SearchCategory ?? DBNull.Value)
                };
                var results = _context.Subjects.FromSqlRaw("EXEC usp_GetSubjectSearchSuggestions @term, @category", parameters)
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
        public RepositoryResponse<bool> CheckSubjectAllocationStatus(long id)
        {
            var response = new RepositoryResponse<bool>();
            var subjectID = new SqlParameter("@SubjectID", id);
            var result = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };

            try
            {
                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_CheckSubjectAllocation @SubjectID, @Result OUTPUT", subjectID, result);


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
