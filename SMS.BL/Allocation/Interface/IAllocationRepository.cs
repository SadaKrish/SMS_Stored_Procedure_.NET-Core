using SMS.Model.Teacher_Subject_Allocation;
using SMS.ViewModel.Allocation;
using SMS.ViewModel.AllocationViewModel;
using SMS.ViewModel.RepositoryResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SMS.BL.Allocation.Interface
{
    public interface IAllocationRepository
    {
        RepositoryResponse<IEnumerable<SubjectAllocationDetailViewModel>> GetAllSubjectAllocations();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectAllocationId"></param>
        /// <returns></returns>
        Teacher_Subject_AllocationBO GetSubjectAllocationById(long subjectAllocationId);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEnabledTeachersList();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetEnabledSubjectList();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allocation"></param>
        /// <returns></returns>
        RepositoryResponse<bool> UpsertSubjectAllocation(Teacher_Subject_AllocationBO allocation);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DeleteSubjectAllocation(long id);
        RepositoryResponse<IEnumerable<Teacher_Subject_AllocationBO>> SearchSubjectAllocations(SearchViewModel searchModel);
        RepositoryResponse<IEnumerable<Teacher_Subject_AllocationBO>> GetSubjectAllocationByTerm(SearchViewModel searchModel);
        RepositoryResponse<IEnumerable<StudentAllocationGroupedViewModel>> GetAllStudentAllocations(bool? status = null);
        RepositoryResponse<bool> DeleteStudentAllocation(long id);
    }
}
