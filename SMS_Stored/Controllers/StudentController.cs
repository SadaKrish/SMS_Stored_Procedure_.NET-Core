using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.BL.Student.Interface;
using SMS.Model.Student;
using SMS.ViewModel.ErrorResponse;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.ViewModel.Student;


namespace SMS_Stored.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;

        public StudentController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }
        /// <summary>
        /// Index page view
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
           
            return View();
        }
        /// <summary>
        /// Get the student details
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetStudents(string status = "all")
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = new RepositoryResponse<IEnumerable<StudentBO>>();
                bool? isEnabled = null;
                if (status.ToLower() == "active")
                {
                    isEnabled = true;
                }
                else if (status.ToLower() == "inactive")
                {
                    isEnabled = false;
                }

                response = _studentRepository.GetStudents(isEnabled);

                if (response.Success && response.Data != null && response.Data.Any())
                {
                    return Json(new { success = true, data = response.Data });
                }
                else
                {
                    errorResponse.Messages.Add(string.Format(response.Messages.ToString()));
                    return new JsonResult(errorResponse);
                }
            }
            catch (Exception)
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Students"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }



        /// <summary>
        /// add or edit student details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AddOrEditStudent(long id = 0)
        {
            if (id == 0)
            {
                
                return PartialView("_AddOrEdit", new StudentBO());
            }
            else
            {
                // Edit existing student
                var student = _studentRepository.GetStudentByID(id);
                if (student == null)
                {
                    return NotFound();
                }
                return PartialView("_AddOrEdit", student);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrEditStudent(StudentBO student)
        {
            if (ModelState.IsValid)
            {
                string message;

                // Save or update the student using the repository method
                if (_studentRepository.SaveStudent(student, out message))
                {
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = message });
                }
            }
            else
            {
                // Return validation errors
                var errors = ModelState.Where(ms => ms.Value.Errors.Any())
                               .Select(ms => new
                               {
                                   Key = ms.Key,
                                   Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                               });

                return Json(new { success = false, errors });
            }
        }

        /// <summary>
        /// delete the student details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteStudent(long id)
        {
            string message;
            bool requiresConfirmation;

            var success = _studentRepository.DeleteStudent(id, out message, out requiresConfirmation);

            if (success)
            {
                return Json(new { success = true, message = message });
            }
            else
            {
                return Json(new { success = false, requiresConfirmation = requiresConfirmation, message = message });
            }
        }

        /// <summary>
        /// check the existence of registration number
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckStudentRegNoExists(string regNo)
        {
            bool exists = _studentRepository.DoesStudentRegNoExist(regNo);
            return Json(new { exists });
        }
        /// <summary>
        /// check the existence of display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckStudentDisplayNameExists(string displayName)
        {
            bool exists = _studentRepository.DoesStudentDisplayNameExist(displayName);
            return Json(new { exists });
        }
        /// <summary>
        /// Check the existence of emial id
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckStudentEmailExists(string email)
        {
            bool exists = _studentRepository.DoesStudentEmailExist(email);
            return Json(new { exists });
        }
        /// <summary>
        /// Check the student allocation status
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        public JsonResult CheckStudentAllocationStatus(long studentID)
        {
            bool isAllocated = _studentRepository.CheckStudentAllocationStatus(studentID);
            return Json(new { isAllocated = isAllocated });
        }
        /// <summary>
        /// Change the status of a student
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleEnableStudent(long id)
        {
            string message;
            var success = _studentRepository.ToggleStudentEnable(id, out message);

            if (success)
            {
                return Json(new { success = true, message = message });
            }
            else
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Search based on categories
        /// </summary>
        /// <param name="searchText"></param>
        /// <param name="searchCategory"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchStudents(string searchText, string searchCategory)
      {
            try
            {
                var searchResults = _studentRepository.SearchStudents(searchText, searchCategory);
                return Json(new { success = true, data = searchResults });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
