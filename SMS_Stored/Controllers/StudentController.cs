﻿/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Logging;
using NLog;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SMS.BL.Student.Interface;
using SMS.Model.Student;
using SMS.ViewModel.ErrorResponse;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.ViewModel.Student;
using System.Diagnostics.Eventing.Reader;


namespace SMS_Stored.Controllers
{
    public class StudentController : Controller
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ILogger<StudentController>_logger;
        

        public StudentController(IStudentRepository studentRepository,ILogger<StudentController> logger)
        {
            _studentRepository = studentRepository;
            _logger = logger;
        }
        /// <summary>
        /// Index page view
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            _logger.LogInformation("Index page view requested");
            var viewModel = new StudentViewModel
            {
                StudentList = new List<StudentBO>(),
                SearchView = new SearchViewModel() 
            };
            return View(viewModel);
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
                _logger.LogInformation("GetStudents method called with status:{ Status}", status);
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
                    var viewModel = new StudentViewModel
                    {
                        StudentList = response.Data,
                        SearchView = new SearchViewModel()

                    };
                    return PartialView("_StudentList", viewModel);
                }
                else
                {
                    errorResponse.Messages.Add(string.Format(response.Messages.ToString()));
                    return new JsonResult(errorResponse);
                }
            }
            catch 
            {
                _logger.LogError("Error occurred while getting students");
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
        public ActionResult UpsertStudent(long id = 0)
        {
            if (id == 0)
            {
                _logger.LogInformation("UpsertStudent method called to add a new Student");
                return PartialView("_Upsert", new StudentBO());
            }
            else
            {
                // Edit existing student
                _logger.LogWarning("UpsertStudent method called to edit student with ID: {ID}", id);
                var response = _studentRepository.GetStudentByID(id);
                if (!response.Success || response.Data == null)
                {
                    return NotFound();
                }
                return PartialView("_Upsert", response.Data);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpsertStudent(StudentBO student)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = new RepositoryResponse<bool>();
                response = _studentRepository.UpsertStudent(student);

                if (response.Success)
                {
                    return Json(new { success = true, message = response.Messages });
                   
                }
                else
                {
                    _logger.LogError("User attempt to submit form without filling necessary fields");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch
            {
                
                errorResponse.Messages.Add(string.Format(StaticMessages.Fill_Form));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
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
            var errorResponse = new ErrorResponse();
           
           // bool requiresConfirmation;
            try
            {
                _logger.LogInformation("Delete Student method called to delete student with ID: {ID}", id);
                var response = new RepositoryResponse<bool>();
               // string message;

                response = _studentRepository.DeleteStudent(id);

                if (response.Success)
                {
                    return Json(new { success = true, message = response.Messages });
                }
                else
                {
                    _logger.LogWarning("Delete method called to delete student with ID: {ID}", id);
                    
                    return Json(new { success = false,  message = response.Messages });
                }

            }
            catch 
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Students"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
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
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _studentRepository.DoesStudentRegNoExist(regNo);

                if (response.Success)
                {
                    return Json(new { exists = response.Data });
                }
                else
                {
                    _logger.LogError("The registration number entered is alredy exists.");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch 
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Student Registration Number"));
               // errorResponse.Messages.Add(ex.Message); 

                return Json(new { success = false, message = errorResponse.Messages });
            }
        }

        /// <summary>
        /// check the existence of display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckStudentDisplayNameExists(string displayName)
        {
            var errorResponse= new ErrorResponse();
            try
            {
                var response = _studentRepository.DoesStudentDisplayNameExist(displayName);
                if (response.Success)
                {
                    return Json(new { exists = response.Data });
                }
                else
                {
                    _logger.LogError("The display name entered is alredy exists.");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch (Exception ex)
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Student Display name"));
                errorResponse.Messages.Add(ex.Message); 

                return Json(new { success = false, message = errorResponse.Messages });
            }
        }
        /// <summary>
        /// Check the existence of emial id
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckStudentEmailExists(string email)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _studentRepository.DoesStudentEmailExist(email);
                if (response.Success)
                {
                    return Json(new { exists = response.Data });
                }
                else
                {
                    _logger.LogError("The email id entered is alredy exists.");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Student Email"));
                return Json(new { success = false, message = errorResponse.Messages });
            }
        }
        /// <summary>
        /// Check the student allocation status
        /// </summary>
        /// <param name="studentID"></param>
        /// <returns></returns>
        public JsonResult CheckStudentAllocationStatus(long studentID)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _studentRepository.CheckStudentAllocationStatus(studentID);
                if (response.Success)
                {
                    return Json(new { exists = response.Data });
                }
                else
                {
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Student Allocation"));
                return Json(new { success = false, message = errorResponse.Messages });
            }

        }
           
       
        /// <summary>
        /// Change the status of a student
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleEnableStudent(long id)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _studentRepository.ToggleStudentEnable(id);
                if (response.Success)
                {
                    _logger.LogError("The status of student {id} is updated",id);
                    return Json(new { success = true, message = response.Messages });
                }
                else
                {
                    _logger.LogError("The status does not updated");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch 
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Student Status"));
                return Json(new { success = false, message = errorResponse.Messages });
            }

           
        }
       /// <summary>
       /// Search filter
       /// </summary>
       /// <param name="model"></param>
       /// <returns></returns>
        [HttpPost]
        public ActionResult SearchStudents(SearchViewModel model)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _studentRepository.SearchStudents(model);
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    var viewModel = new StudentViewModel
                    {
                        StudentList = response.Data,
                        SearchView = new SearchViewModel()

                    };
                    return PartialView("_StudentList", viewModel);
                }
                else
                {
                    _logger.LogError("No Students found");
                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                _logger.LogError("Error in search students");
                errorResponse.Messages.Add(string.Format(StaticMessages.Data_Not_Found));
               // errorResponse.Messages.Add(ex.Message); 

                return Json(new { success = false, message = errorResponse.Messages });
            }
        }

        

    }
}
