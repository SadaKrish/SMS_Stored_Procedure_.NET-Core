/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using System.Data;
using NLog;
using Microsoft.AspNetCore.Mvc;
using SMS.BL.Student.Interface;
using SMS.Model.Student;
using SMS.ViewModel.ErrorResponse;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.ViewModel.Student;
using System.Diagnostics.Eventing.Reader;
using ClosedXML.Excel;
using Rotativa.AspNetCore;
using System.Text.RegularExpressions;


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
                    _logger.LogError("User attempt to submit form without filling necessary fields or existence data");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch(Exception ex)
            {
                
                errorResponse.Messages.Add(string.Format(StaticMessages.Fill_Form));
                _logger.LogError(ex, "An error occurred while add or edit a student ");
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
                _logger.LogDebug("Checked the registration number existence");
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
            catch (Exception ex)
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Student Registration Number"));
                _logger.LogError(ex, "An error occurred while checking the registration number: {regNo}", regNo);

                return Json(new { success = false, message = errorResponse.Messages, exception = ex.Message });
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
                _logger.LogDebug("Checked the display name existence");
                var response = _studentRepository.DoesStudentDisplayNameExist(displayName);
                if (response.Success)
                {
                    _logger.LogDebug("Checked the display name existence");
                    return Json(new { exists = response.Data });
                }
                else
                {
                    _logger.LogWarning("The display name entered is alredy exists.");
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
                _logger.LogDebug("Checked the Email Id existence");
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetSearchSuggestions(string term, string category)
        {
            var searchModel = new SearchViewModel
            {
                SearchText = term,
                SearchCategory = category
            };

            var response = _studentRepository.GetStudentsByTerm(searchModel);

            if (response.Success)
            {
                var suggestions = response.Data.Select(s => new
                {
                    label = category switch
                    {
                        "StudentRegNo" => s.StudentRegNo,
                        "FirstName" => s.FirstName,
                        "LastName" => s.LastName,
                        "DisplayName" => s.DisplayName,
                        _ => s.StudentRegNo 
                    },
                    value = category switch
                    {
                        "StudentRegNo" => s.StudentRegNo,
                        "FirstName" => s.FirstName,
                        "LastName" => s.LastName,
                        "DisplayName" => s.DisplayName,
                        _ => s.StudentRegNo // Default case
                    },
                    data = s
                }).ToList();

                return Json(suggestions);
            }
            else
            {
                return Json(new { success = false, message = string.Join(", ", response.Messages) });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public ActionResult ExportToExcel(string status = "all")
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

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Students");
                    var headerRow = worksheet.Row(1);
                    worksheet.Cell(1, 1).Value = "Student Reg No";
                    worksheet.Cell(1, 2).Value = "First Name";
                    worksheet.Cell(1, 3).Value = "Middle Name";
                    worksheet.Cell(1, 4).Value = "Last Name";
                    worksheet.Cell(1, 5).Value = "Display Name";
                    worksheet.Cell(1, 6).Value = "Email";
                    worksheet.Cell(1, 7).Value = "Gender";
                    worksheet.Cell(1, 8).Value = "DOB";
                    worksheet.Cell(1, 9).Value = "Address";
                    worksheet.Cell(1, 10).Value = "Contact No";
                    worksheet.Cell(1, 11).Value = "Is Enable";

                    headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    headerRow.Style.Font.Bold= true;

                    int row = 2;
                    foreach (var student in response.Data)
                    {
                        worksheet.Cell(row, 1).Value = student.StudentRegNo;
                        worksheet.Cell(row, 2).Value = student.FirstName;
                        worksheet.Cell(row, 3).Value = student.MiddleName;
                        worksheet.Cell(row, 4).Value = student.LastName;
                        worksheet.Cell(row, 5).Value = student.DisplayName;
                        worksheet.Cell(row, 6).Value = student.Email;
                        worksheet.Cell(row, 7).Value = student.Gender;
                        worksheet.Cell(row, 8).Value = student.DOB;
                        worksheet.Cell(row, 9).Value = student.Address;
                        worksheet.Cell(row, 10).Value = student.ContactNo;
                        worksheet.Cell(row, 11).Value = student.IsEnable ? "Yes" : "No";

                        row++;

                    }
                    string fileName = $"StudentList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        byte[] byteArray = stream.ToArray();
                        return File(byteArray, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                    }
                }
            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Students"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public IActionResult ExportToPdf(string status = "all")
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

            var response = _studentRepository.GetStudents(isEnabled);
            var students = response.Data;
            var studentViewModel = new StudentViewModel
            {
                StudentList = students,
                SearchView = new SearchViewModel() 
            };
            return new ViewAsPdf("_StudentListPdf", studentViewModel)
            {
                FileName = $"StudentList_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking --print-media-type --viewport-size 1280x1024"
            };
        }


    }
}
