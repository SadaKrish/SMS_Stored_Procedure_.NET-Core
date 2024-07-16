using SMS.Model.Student_Subject_Teacher_Allocation;
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
        /// <summary>
        /// Get all subject allocation
        /// </summary>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<SubjectAllocationDetailViewModel>> GetAllSubjectAllocations();
        /// <summary>
        /// Get subject allocation by id
        /// </summary>
        /// <param name="subjectAllocationId"></param>
        /// <returns></returns>
        Teacher_Subject_AllocationBO GetSubjectAllocationById(long subjectAllocationId);
        /// <summary>
        /// Get the enabled teacher list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetEnabledTeachersList();
        /// <summary>
        /// Get the enabled subject list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetEnabledSubjectList();
        /// <summary>
        /// add or edit subject allocation
        /// </summary>
        /// <param name="allocation"></param>
        /// <returns></returns>
        RepositoryResponse<bool> UpsertSubjectAllocation(Teacher_Subject_AllocationBO allocation);
        /// <summary>
        /// delete subject allocation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DeleteSubjectAllocation(long id);
        /// <summary>
        /// Search option in subject allocation
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<SubjectAllocationDetailViewModel>> SearchSubjectAllocations(SearchViewModel searchModel);
        /// <summary>
        /// Get all student allocation list
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<StudentAllocationGroupedViewModel>> GetAllStudentAllocations(bool? status = null);
        /// <summary>
        /// Add student allocation
        /// </summary>
        /// <param name="allocation"></param>
        /// <returns></returns>
        RepositoryResponse<bool> AddStudentAllocation(Student_Subject_Teacher_AllocationBO allocation);
        /// <summary>
        /// Get teacher list based on student id
        /// </summary>
        /// <param name="subjectID"></param>
        /// <returns></returns>
        IEnumerable<object> GetTeachersBySubjectID(long subjectID);
        /// <summary>
        /// Delete student allocation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RepositoryResponse<bool> DeleteStudentAllocation(long id);
        /// <summary>
        /// get enabled studnet list
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetEnabledStudentList();
        /// <summary>
        /// Get tecaher list from allocation
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetTeacherListFromAllocation();
        /// <summary>
        /// Get teacher subject lits from allocation
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetAllSubjectsFromAllocation();
        /// <summary>
        /// search option
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        RepositoryResponse<IEnumerable<StudentAllocationGroupedViewModel>> SearchStudentAllocations(SearchViewModel searchModel);
    }
}
