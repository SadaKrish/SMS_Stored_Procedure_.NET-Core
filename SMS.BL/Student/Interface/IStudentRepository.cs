using SMS.Model.Student;
using SMS.ViewModel.RepositoryResponse;
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
        StudentBO GetStudentByID(long studentID);
        /// <summary>
        /// Add or edit student
        /// </summary>
        /// <param name="student"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool SaveStudent(StudentBO student, out string msg);
        /// <summary>
        /// check the registration number existence
        /// </summary>
        /// <param name="studentRegNo"></param>
        /// <returns></returns>
        bool DoesStudentRegNoExist(string studentRegNo);
        /// <summary>
        ///Check Student display name existence
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        bool DoesStudentDisplayNameExist(string displayName);
        /// <summary>
        /// Check student email existence
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool DoesStudentEmailExist(string email);
        /// <summary>
        /// Delete the student details
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="message"></param>
        /// <param name="requiresConfirmation"></param>
        /// <returns></returns>
        bool DeleteStudent(long studentId, out string message, out bool requiresConfirmation);
        /// <summary>
        /// Status change of a student
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool ToggleStudentEnable(long studentId, out string message);
        /// <summary>
        /// Search based on categories
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        IEnumerable<StudentBO> SearchStudents(string searchText, string searchCategory);
        /// <summary>
        /// Check the whether student is allocated for subject
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool CheckStudentAllocationStatus(long id);
    }
}
