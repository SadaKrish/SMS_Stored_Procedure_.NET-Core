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
        public StudentBO GetStudentByID(long studentID)
        {
            var student = _context.Students.FromSqlRaw("EXEC Get_StudentById @StudentId = {0}", studentID).ToList().FirstOrDefault();
            return student;

        }
        /// <summary>
        /// Add or edit student details
        /// </summary>
        /// <param name="student"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SaveStudent(StudentBO student, out string msg)
        {
            msg = "";

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

                _context.Database.ExecuteSqlRaw("EXEC dbo.usp_AddOrEdit_Student @StudentID, @StudentRegNo, @FirstName, @MiddleName, @LastName, @DisplayName, @Email, @Gender, @DOB, @Address, @ContactNo, @IsEnable, @Result OUTPUT",
                                                 studentIDParam, studentRegNoParam, firstNameParam, middleNameParam, lastNameParam,
                                                 displayNameParam, emailParam, genderParam, dobParam, addressParam, contactNoParam, isEnableParam, resultParam);

                int result = (int)resultParam.Value;

                if (result > 0)
                {
                    msg = student.StudentID == null ? "Student added successfully!" : "Student details updated successfully";
                    return true;
                }
                else
                {
                    msg = "Failed to save student details.";
                    return false;
                }
            }
            catch (SqlException ex)
            {
                msg = $"SQL Error: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                msg = $"Error: {ex.Message}";
                return false;
            }
        }
        /// <summary>
        /// Check the student regsitration number is exsit or not
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        public bool DoesStudentRegNoExist(string regNo)
        {
            var stRegNo = new SqlParameter("@RegNo", regNo);
            //  var StdID = new SqlParameter("@StdID", studentID);
            var result = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };


            _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckStudentRegNoExists(@RegNo)", stRegNo, result);

            return (bool)result.Value;
        }
        /// <summary>
        /// check the existence of display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public bool DoesStudentDisplayNameExist(string displayName)
        {
            var stDisplayName = new SqlParameter("@DisplayName", displayName);

            var result = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };


            _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckDisplayNameExists(@DisplayName)", stDisplayName, result);

            return (bool)result.Value;
        }
        /// <summary>
        /// check the existence of student email ID
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        public bool DoesStudentEmailExist(string email)
        {
            var stEmail = new SqlParameter("@Email", email);

            var result = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };


            _context.Database.ExecuteSqlRaw("SELECT @Result = dbo.fn_CheckEmailExists(@Email)", stEmail, result);

            return (bool)result.Value;
        }
        /// <summary>
        /// delete the student details
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public bool DeleteStudent(long studentId, out string message, out bool requiresConfirmation)
        {
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
                _context.Database.ExecuteSqlRaw(
                    "EXEC dbo.usp_DeleteStudent @StudentID, @Message OUTPUT, @RequiresConfirmation OUTPUT",
                    studentIdParam, messageParam, requiresConfirmationParam
                );

                message = (string)messageParam.Value;
                requiresConfirmation = (bool)requiresConfirmationParam.Value;

                return !requiresConfirmation;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                requiresConfirmation = false;
                return false;
            }
        }
        /// <summary>
        /// Change the status of a student
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ToggleStudentEnable(long studentId, out string message)
        {
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

            _context.Database.ExecuteSqlRaw("EXEC dbo.usp_ToggleStudentEnable @StudentID, @Message OUTPUT, @Success OUTPUT",
                                              studentIdParam, messageParam, successParam);

            message = (string)messageParam.Value;
            return (bool)successParam.Value;
        }
        /// <summary>
        /// Search based on category
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        public IEnumerable<StudentBO> SearchStudents(string searchText, string searchCategory)
        {
            var searchTextParam = new SqlParameter("@SearchText", searchText);
            var searchCategoryParam = new SqlParameter("@SearchCategory", searchCategory);

            var students = _context.Students
                .FromSqlRaw("EXEC dbo.usp_SearchStudents @SearchText, @SearchCategory", searchTextParam, searchCategoryParam)
                .ToList();

            return students;
        }
        /// <summary>
        /// check whether student is allocated for a subject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckStudentAllocationStatus(long id)
        {
            var stdID = new SqlParameter("@StudentID", id);
            var result = new SqlParameter
            {
                ParameterName = "@Result",
                SqlDbType = SqlDbType.Bit,
                Direction = ParameterDirection.Output
            };

            // Execute the SQL command
            _context.Database.ExecuteSqlRaw("EXEC dbo.usp_CheckStudentAllocation @StudentID, @Result OUTPUT", stdID, result);

            return (bool)result.Value;
        }



    }






}

