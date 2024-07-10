/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
/// 
using SMS.Model.Student;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.Student;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Student.Interface
{
    public interface IStudentRepository
    {
        /// <summary>
        /// Get all students
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<StudentBO>> GetStudents(bool? isEnable);
        /// <summary>
        /// Get student by ID
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        RepositoryResponse<StudentBO> GetStudentByID(long studentId);
        /// <summary>
        /// Student add or edit
        /// </summary>
        /// <param name="student"></param>
        /// <returns></returns>
        RepositoryResponse<bool> UpsertStudent(StudentBO student);
        /// <summary>
        /// Check the existence of registration no
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesStudentRegNoExist(string regNo);
        /// <summary>
        /// Check existence of display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesStudentDisplayNameExist(string displayName);
        /// <summary>
        /// Check student email existence
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesStudentEmailExist(string email);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DeleteStudent(long studentId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        RepositoryResponse<bool> ToggleStudentEnable(long studentId);
       /// <summary>
       /// Search student details
       /// </summary>
       /// <param name="searchModel"></param>
       /// <returns></returns>
        RepositoryResponse<IEnumerable<StudentBO>> SearchStudents(SearchViewModel searchModel);
        /// <summary>
        /// Showing Search Options
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<StudentBO>> GetStudentsByTerm(SearchViewModel searchModel);
       /// <summary>
       /// Check whther the student allocated for subject
       /// </summary>
       /// <param name="id"></param>
       /// <returns></returns>
        RepositoryResponse<bool> CheckStudentAllocationStatus(long id);
    }
}
