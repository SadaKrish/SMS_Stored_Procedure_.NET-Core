using Microsoft.AspNetCore.Mvc;
using SMS.BL.Allocation.Interface;

using SMS.ViewModel.Allocation;
using SMS.ViewModel.AllocationViewModel;
using SMS.ViewModel.ErrorResponse;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;

using Microsoft.AspNetCore.Mvc.Rendering;
using SMS.Model.Teacher_Subject_Allocation;
using NuGet.Protocol.Core.Types;


namespace SMS_Stored.Controllers
{
    public class AllocationController : Controller
    {
        private readonly IAllocationRepository _allocationRepository;
        private readonly ILogger<AllocationController> _logger;

        public AllocationController(IAllocationRepository allocationRepository, ILogger<AllocationController> logger)
        {
            _allocationRepository = allocationRepository;
            _logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult SubjectAllocation()
        {
           return View();
        }
        public IActionResult StudentAllocation()
        {
            return View();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSubjectAllocations()
        {
            var errorResponse = new ErrorResponse();
            try
            {
                _logger.LogInformation("GetSubjectAllocations method called");
                var response = _allocationRepository.GetAllSubjectAllocations();

                if (response.Success && response.Data != null && response.Data.Any())
                {
                    return PartialView("_SubjectAllocationList", response.Data);
                }
                else
                {
                    errorResponse.Messages.Add("No data available or failed to retrieve data.");
                    if (response.Messages.Any())
                    {
                        errorResponse.Messages.AddRange(response.Messages);
                    }
                    return Json(new { success = false, messages = errorResponse.Messages });
                }
            }
            catch (Exception ex)
            {              
                _logger.LogError(ex, "Error occurred while getting subject allocations");
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subject Allocations"));
                return Json(new { success = false, messages = errorResponse.Messages });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UpsertSubjectAllocation(long id = 0)
        {
            ViewBag.TeacherList = new SelectList(_allocationRepository.GetEnabledTeachersList(), "Value", "Text");
            ViewBag.SubjectList = new SelectList(_allocationRepository.GetEnabledSubjectList(), "Value", "Text");

            if (id == 0)
            {
                return PartialView("_UpsertTeacherSubjectAllocation", new Teacher_Subject_AllocationBO());
            }
            else
            {
                var subjectAllocation = _allocationRepository.GetSubjectAllocationById(id);
                if (subjectAllocation == null)
                {
                    return NotFound();
                }
                return PartialView("_UpsertTeacherSubjectAllocation", subjectAllocation);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpsertSubjectAllocation(Teacher_Subject_AllocationBO subjectAllocation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = _allocationRepository.UpsertSubjectAllocation(subjectAllocation);

                    return Json(new { success = response.Success, message = string.Join(", ", response.Messages) });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = $"An error occurred while processing the request: {ex.Message}" });
                }
            }
            else
            {
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Any())
                    .Select(ms => new
                    {
                        Key = ms.Key,
                        Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    });

                return Json(new { success = false, errors });
            }
        }

        public ActionResult DeleteSubjectAllocation(long id)
        {
            var errorResponse = new ErrorResponse();

            try
            {
                _logger.LogInformation("DeleteSubject method called to delete subject with ID: {ID}", id);
                var response = new RepositoryResponse<bool>();

                response = _allocationRepository.DeleteSubjectAllocation(id);

                if (response.Success)
                {
                    return Json(new { success = true, message = response.Messages });
                }
                else
                {
                    _logger.LogWarning("Delete method called to delete subjectallocation with ID: {ID}", id);

                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Delete_Data, "Subjects Allocation"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }

        [HttpPost]
        public ActionResult SearchSubjectAllocation(SearchViewModel model)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _allocationRepository.SearchSubjectAllocations(model);
                if (response.Success && response.Data != null && response.Data.Any())
                {

                    return PartialView("_SubjectAllocationList", response.Data);
                }
                else
                {
                    _logger.LogError("No allocations found");
                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                _logger.LogError("Error in search allocations");
                errorResponse.Messages.Add(string.Format(StaticMessages.Data_Not_Found));
                // errorResponse.Messages.Add(ex.Message); 

                return Json(new { success = false, message = errorResponse.Messages });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetSearchSuggestions(SearchViewModel searchModel)
        {
           

            var suggestions = _allocationRepository.GetSubjectAllocationByTerm(searchModel);

            return Json(suggestions);
        }
        /**********************************************Student Allocation************************************/
        /// <summary>
        /// /
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStudentAllocations(string status = "all")
        {
            var errorResponse = new ErrorResponse();
            try
            {
               
                bool? isEnabled = null;
                if (status.ToLower() == "active")
                {
                    isEnabled = true;
                }
                else if (status.ToLower() == "inactive")
                {
                    isEnabled = false;
                }
                 var response = _allocationRepository.GetAllStudentAllocations(isEnabled);
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    
                    return PartialView("_StudentAllocationList", response.Data);
                }
                else
                {
                    errorResponse.Messages.Add(string.Format(response.Messages.ToString()));
                    return new JsonResult(errorResponse);
                }
            }
            catch (Exception ex)
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Students Allocation"));
                return new JsonResult(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }
        [HttpGet]
        public IActionResult UpsertStudentAllocation(long id = 0)
        {
            ViewBag.TeacherList = new SelectList(_allocationRepository.GetEnabledTeachersList(), "Value", "Text");
            ViewBag.SubjectList = new SelectList(_allocationRepository.GetEnabledSubjectList(), "Value", "Text");

            if (id == 0)
            {
                return PartialView("_UpsertTeacherSubjectAllocation", new Teacher_Subject_AllocationBO());
            }
            else
            {
                var subjectAllocation = _allocationRepository.GetSubjectAllocationById(id);
                if (subjectAllocation == null)
                {
                    return NotFound();
                }
                return PartialView("_UpsertTeacherSubjectAllocation", subjectAllocation);
            }
        }
        public ActionResult DeleteStudentAllocation(long id)
        {
            var errorResponse = new ErrorResponse();

            try
            {
                _logger.LogInformation("DeleteSubject method called to delete subject with ID: {ID}", id);
                var response = new RepositoryResponse<bool>();

                response = _allocationRepository.DeleteStudentAllocation(id);

                if (response.Success)
                {
                    return Json(new { success = true, message = response.Messages });
                }
                else
                {
                    _logger.LogWarning("Delete method called to delete subjectallocation with ID: {ID}", id);

                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Delete_Data, "Subjects Allocation"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }
    }
}
