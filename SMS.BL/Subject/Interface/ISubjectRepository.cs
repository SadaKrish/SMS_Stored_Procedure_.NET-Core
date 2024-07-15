/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using SMS.Model.Subject;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.Subject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.BL.Subject.Interface
{
    public interface ISubjectRepository
    {
        /// <summary>
        /// Get all subjects
        /// </summary>
        /// <param name="isEnable"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<SubjectBO>> GetSubjects(bool? isEnable);
        /// <summary>
        /// Get subject by its id
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        RepositoryResponse<SubjectBO> GetSubjectByID(long subjectId);
        /// <summary>
        /// Add or update subject
        /// </summary>
        /// <param name="subject"></param>
        /// <returns></returns>
        RepositoryResponse<bool> UpsertSubject(SubjectBO subject);
        /// <summary>
        /// check the existnece of subject code
        /// </summary>
        /// <param name="subCode"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesSubjectCodeExist(string subCode);
        /// <summary>
        /// check the existence of subject name
        /// </summary>
        /// <param name="subName"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DoesSubjectNameExist(string subName);
        /// <summary>
        /// Delete Subject
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DeleteSubject(long studentId);
        /// <summary>
        /// Change the status
        /// </summary>
        /// <param name="subjectId"></param>
        /// <returns></returns>
        RepositoryResponse<bool> ToggleSubjectEnable(long subjectId);
        /// <summary>
        /// Search subjects
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<SubjectBO>> SearchSubjects(SearchViewModel searchModel);
        /// <summary>
        /// serach subject sugesstion
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<SubjectBO>> GetSubjectsByTerm(SearchViewModel searchModel);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RepositoryResponse<bool> CheckSubjectAllocationStatus(long id);

    }
}
