/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SMS.BL.Teacher.Interface;
using SMS.Model.Teacher;
using SMS.ViewModel.ErrorResponse;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;

using SMS.ViewModel.Teacher;

namespace SMS_Stored.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly ILogger<TeacherController> _logger;


        public TeacherController(ITeacherRepository teacherRepository, ILogger<TeacherController> logger)
        {
            _teacherRepository = teacherRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var viewModel = new TeacherViewModel
            {
                TeacherList = new List<TeacherBO>(),
                SearchView=new SearchViewModel()
            };
            return View(viewModel);
        }
        /// <summary>
        /// Get All Teachers
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult GetTeachers(string status = "all")
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = new RepositoryResponse<IEnumerable<TeacherBO>>();
                bool? isEnabled = null;
                if (status.ToLower() == "active")
                {
                    isEnabled = true;
                }
                else if (status.ToLower() == "inactive")
                {
                    isEnabled = false;
                }
                response = _teacherRepository.GetTeachers(isEnabled);
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    var viewModel = new TeacherViewModel
                    {
                        TeacherList = response.Data,
                        SearchView = new SearchViewModel()
                    };
                    return PartialView("_TeacherList", viewModel);
                }
                else
                {
                    errorResponse.Messages.Add(string.Format(response.Messages.ToString()));
                    return new JsonResult(errorResponse);
                }
            }
            catch (Exception ex)
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teachers"));
                return new JsonResult(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }

        [HttpGet]
        public ActionResult UpsertTeacher(long id = 0)
        {
            if (id == 0)
            {
               // _logger.LogInformation("UpsertTeacher method called to add a new Teacher");
                return PartialView("_UpsertTeacher", new TeacherBO());
            }
            else
            {
                // Edit existing student
                //_logger.LogWarning("UpsertTeacher method called to edit teacher with ID: {ID}", id);
                var response = _teacherRepository.GetTeacherByID(id);
                if (!response.Success || response.Data == null)
                {
                    return NotFound();
                }
                return PartialView("_UpsertTeacher", response.Data);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpsertTeacher(TeacherBO teacher)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = new RepositoryResponse<bool>();
                response = _teacherRepository.UpsertTeacher(teacher);

                if (response.Success)
                {
                    return Json(new { success = true, message = response.Messages });

                }
                else
                {
                   // _logger.LogError("User attempt to submit form without filling necessary fields or existence data");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch (Exception ex)
            {

                errorResponse.Messages.Add(string.Format(StaticMessages.Fill_Form));
               // _logger.LogError(ex, "An error occurred while add or edit a student ");
                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }
        [HttpPost]
        public ActionResult DeleteTeacher(long id)
        {
            var errorResponse = new ErrorResponse();

            // bool requiresConfirmation;
            try
            {
                _logger.LogInformation("Delete Teacher method called to delete teacher with ID: {ID}", id);
                var response = new RepositoryResponse<bool>();
                // string message;

                response = _teacherRepository.DeleteTeacher(id);

                if (response.Success)
                {
                    return Json(new { success = true, message = response.Messages });
                }
                else
                {
                    _logger.LogWarning("Delete method called to delete teacher with ID: {ID}", id);

                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teachers"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }

        /// <summary>
        /// check the existence of registration number
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckTeacherRegNoExists(string regNo)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                _logger.LogDebug("Checked the registration number existence");
                var response = _teacherRepository.DoesTeacherRegNoExist(regNo);

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
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teacher Registration Number"));
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
        public IActionResult CheckTeacherDisplayNameExists(string displayName)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                _logger.LogDebug("Checked the display name existence");
                var response = _teacherRepository.DoesTeacherDisplayNameExist(displayName);
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
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teacher Display name"));
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
        public IActionResult CheckTeacherEmailExists(string email)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                _logger.LogDebug("Checked the Email Id existence");
                var response = _teacherRepository.DoesTeacherEmailExist(email);
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
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teacher Email"));
                return Json(new { success = false, message = errorResponse.Messages });
            }
        }
        /// <summary>
        /// Check the student allocation status
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        public JsonResult CheckTeacherAllocationStatus(long teacherID)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _teacherRepository.CheckTeacherAllocationStatus(teacherID);
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
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teacher Allocation"));
                return Json(new { success = false, message = errorResponse.Messages });
            }

        }


        /// <summary>
        /// Change the status of a student
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ToggleEnableTeacher(long id)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _teacherRepository.ToggleTeacherEnable(id);
                if (response.Success)
                {
                    _logger.LogWarning("The status of teacher {id} is updated", id);
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
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teacher Status"));
                return Json(new { success = false, message = errorResponse.Messages });
            }


        }
        /// <summary>
        /// Search filter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SearchTeacher(SearchViewModel model)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _teacherRepository.SearchTeachers(model);
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    var viewModel = new TeacherViewModel
                    {
                        TeacherList = response.Data,
                        SearchView = new SearchViewModel()

                    };
                    return PartialView("_TeacherList", viewModel);
                }
                else
                {
                    _logger.LogError("No Teachers found");
                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                _logger.LogError("Error in search teachers");
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

            var response = _teacherRepository.GetTeachersByTerm(searchModel);

            if (response.Success)
            {
                var suggestions = response.Data.Select(s => new
                {
                    label = category switch
                    {
                        "TeacherRegNo" => s.TeacherRegNo,
                        "FirstName" => s.FirstName,
                        "LastName" => s.LastName,
                        "DisplayName" => s.DisplayName,
                        _ => s.TeacherRegNo
                    },
                    value = category switch
                    {
                        "TeacherRegNo" => s.TeacherRegNo,
                        "FirstName" => s.FirstName,
                        "LastName" => s.LastName,
                        "DisplayName" => s.DisplayName,
                        _ => s.TeacherRegNo // Default case
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

                var response = new RepositoryResponse<IEnumerable<TeacherBO>>();
                bool? isEnabled = null;
                if (status.ToLower() == "active")
                {
                    isEnabled = true;
                }
                else if (status.ToLower() == "inactive")
                {
                    isEnabled = false;
                }

                response = _teacherRepository.GetTeachers(isEnabled);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Teachers");
                    var headerRow = worksheet.Row(1);
                    worksheet.Cell(1, 1).Value = "Teacher Reg No";
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
                    headerRow.Style.Font.Bold = true;

                    int row = 2;
                    foreach (var student in response.Data)
                    {
                        worksheet.Cell(row, 1).Value = student.TeacherRegNo;
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
                    string fileName = $"TeacherList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Teachers"));

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

            var response = _teacherRepository.GetTeachers(isEnabled);
            var teachers = response.Data;
            var teacherViewModel = new TeacherViewModel
            {
                TeacherList = teachers,
                SearchView = new SearchViewModel()
            };
            return new ViewAsPdf("_TeacherListPdf", teacherViewModel)
            {
                FileName = $"StudentList_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking --print-media-type --viewport-size 1280x1024"
            };
        }
    }
}
