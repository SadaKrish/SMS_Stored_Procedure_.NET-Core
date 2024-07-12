using SMS.Model.Teacher;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.Teacher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Teacher.Interface
{
    public interface ITeacherRepository
    {
        /// <summary>
        /// Get all teachers
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<TeacherBO>> GetTeachers(bool? isEnable);
        /// <summary>
        /// Get Teacher by ID
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        RepositoryResponse<TeacherBO> GetTeacherByID(long teacherId);
        /// <summary>
        /// Add or Edit Teacher
        /// </summary>
        /// <param name="teacher"></param>
        /// <returns></returns>
        RepositoryResponse<bool> UpsertTeacher(TeacherBO teacher);
        /// <summary>
        /// Check the existence of reg no
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesTeacherRegNoExist(string regNo);
        /// <summary>
        /// Check the existence of Teacher display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesTeacherDisplayNameExist(string displayName);
        /// <summary>
        /// Check the email existence
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesTeacherEmailExist(string email);
        /// <summary>
        /// Delete teacher
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DeleteTeacher(long teacherId);
        /// <summary>
        /// Change status of teacher
        /// </summary>
        /// <param name="teacherId"></param>
        /// <returns></returns>
        RepositoryResponse<bool> ToggleTeacherEnable(long teacherId);
        /// <summary>
        /// Search teachers
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<TeacherBO>> SearchTeachers(SearchViewModel searchModel);
        /// <summary>
        /// Search  Autofill
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<TeacherBO>> GetTeachersByTerm(SearchViewModel searchModel);
        /// <summary>
        /// Check the teacher allocation status
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RepositoryResponse<bool> CheckTeacherAllocationStatus(long id);

    }
}
