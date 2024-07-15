/// <summary>
///
/// </summary>
/// <author>Sadakshini</author>
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SMS.BL.Subject.Interface;
using SMS.Model.Subject;
using SMS.ViewModel.ErrorResponse;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.ViewModel.Subject;


namespace SMS_Stored.Controllers
{
    public class SubjectController : Controller
    {
        private readonly ISubjectRepository _subjectRepository;
        private readonly ILogger<SubjectController> _logger;


        public SubjectController(ISubjectRepository subjectRepository, ILogger<SubjectController> logger)
        {
            _subjectRepository = subjectRepository;
            _logger = logger;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            _logger.LogInformation("Index page view requested");
            var viewModel = new SubjectViewModel
            {
                SubjectList = new List<SubjectBO>(),
                SearchView = new SMS.ViewModel.Subject.SearchViewModel()
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult GetSubjects(string status = "all")
        {
            var errorResponse = new ErrorResponse();
            try
            {
                _logger.LogInformation("GetSubjects method called with status:{ Status}", status);
                var response = new RepositoryResponse<IEnumerable<SubjectBO>>();
                bool? isEnabled = null;
                if (status.ToLower() == "active")
                {
                    isEnabled = true;
                }
                else if (status.ToLower() == "inactive")
                {
                    isEnabled = false;
                }

                response = _subjectRepository.GetSubjects(isEnabled);

                if (response.Success && response.Data != null && response.Data.Any())
                {
                    var viewModel = new SubjectViewModel
                    {
                        SubjectList = response.Data,
                        SearchView = new SearchViewModel()

                    };
                    return PartialView("_SubjectList", viewModel);
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
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subjects"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult UpsertSubject(long id = 0)
        {
            if (id == 0)
            {
                // _logger.LogInformation("UpsertTeacher method called to add a new Teacher");
                return PartialView("_UpsertSubject", new SubjectBO());
            }
            else
            {
                // Edit existing student
                //_logger.LogWarning("UpsertTeacher method called to edit teacher with ID: {ID}", id);
                var response = _subjectRepository.GetSubjectByID(id);
                if (!response.Success || response.Data == null)
                {
                    return NotFound();
                }
                return PartialView("_UpsertSubject", response.Data);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpsertSubject(SubjectBO subject)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = new RepositoryResponse<bool>();
                response = _subjectRepository.UpsertSubject(subject);

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
            catch (Exception ex)
            {

                errorResponse.Messages.Add(string.Format(StaticMessages.Fill_Form));
                 _logger.LogError(ex, "An error occurred while add or edit a subject ");
                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }
        [HttpPost]
        public ActionResult DeleteSubject(long id)
        {
            var errorResponse = new ErrorResponse();

            // bool requiresConfirmation;
            try
            {
                _logger.LogInformation("DeleteSubject method called to delete subject with ID: {ID}", id);
                var response = new RepositoryResponse<bool>();
                // string message;

                response = _subjectRepository.DeleteSubject(id);

                if (response.Success)
                {
                    return Json(new { success = true, message = response.Messages });
                }
                else
                {
                    _logger.LogWarning("Delete method called to delete subject with ID: {ID}", id);

                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subjects"));

                return Json(new { success = errorResponse.Success, message = errorResponse.ErrorMessages });
            }
        }

        /// <summary>
        /// check the existence of registration number
        /// </summary>
        /// <param name="regNo"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckSubjectCodeExists(string subCode)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                _logger.LogDebug("Checked the subject code existence");
                var response = _subjectRepository.DoesSubjectCodeExist(subCode);

                if (response.Data==true && response.Success)
                {
                    _logger.LogWarning("The subject code entered is alredy exists.");
                    return Json(new { exists = response.Data });
                }
                else
                {
                    
                    return Json(new { exists = response.Data });
                }
            }
            catch (Exception ex)
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subject Code"));
                _logger.LogError(ex, "An error occurred while checking the subject code: {subCode}", subCode);

                return Json(new { success = false, message = errorResponse.Messages, exception = ex.Message });
            }
        }

        /// <summary>
        /// check the existence of display name
        /// </summary>
        /// <param name="displayName"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult CheckSubjectNameExists(string subName)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                _logger.LogDebug("Checked the name existence");
                var response = _subjectRepository.DoesSubjectNameExist(subName);
                if (response.Success)
                {
                    _logger.LogDebug("Checked the  name existence");
                    return Json(new { exists = response.Data });
                }
                else
                {
                    _logger.LogWarning("The  name entered is alredy exists.");
                    return Json(new { success = false, message = response.Messages });
                }
            }
            catch (Exception ex)
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Subject name"));
                errorResponse.Messages.Add(ex.Message);

                return Json(new { success = false, message = errorResponse.Messages });
            }
        }
        /// <summary>
        /// Check the student allocation status
        /// </summary>
        /// <param name="teacherID"></param>
        /// <returns></returns>
        public JsonResult CheckSubjectAllocationStatus(long subjectID)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _subjectRepository.CheckSubjectAllocationStatus(subjectID);
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
        public ActionResult ToggleEnableSubject(long id)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _subjectRepository.ToggleSubjectEnable(id);
                if (response.Success)
                {
                    _logger.LogWarning("The status of subject {id} is updated", id);
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
        public ActionResult SearchSubjects(SearchViewModel model)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _subjectRepository.SearchSubjects(model);
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    var viewModel = new SubjectViewModel
                    {
                        SubjectList = response.Data,
                        SearchView = new SearchViewModel()

                    };
                    return PartialView("_SubjectList", viewModel);
                }
                else
                {
                    _logger.LogError("No Subjects found");
                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
                _logger.LogError("Error in search subjects");
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

            var response = _subjectRepository.GetSubjectsByTerm(searchModel);

            if (response.Success)
            {
                var suggestions = response.Data.Select(s => new
                {
                    label = category switch
                    {
                        "SubjectCode" => s.SubjectCode,
                        "Name" => s.Name,
                        _ => s.SubjectCode
                    },
                    value = category switch
                    {
                        "SubjectCode" => s.SubjectCode,
                        "Name" => s.Name,
                        _ => s.SubjectCode 
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

                var response = new RepositoryResponse<IEnumerable<SubjectBO>>();
                bool? isEnabled = null;
                if (status.ToLower() == "active")
                {
                    isEnabled = true;
                }
                else if (status.ToLower() == "inactive")
                {
                    isEnabled = false;
                }

                response = _subjectRepository.GetSubjects(isEnabled);

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Subjects");
                    var headerRow = worksheet.Row(1);
                    worksheet.Cell(1, 1).Value = "Subject Code";
                    worksheet.Cell(1, 2).Value = "Name";
                    worksheet.Cell(1, 11).Value = "Is Enable";

                    headerRow.Style.Fill.BackgroundColor = XLColor.LightBlue;
                    headerRow.Style.Font.Bold = true;

                    int row = 2;
                    foreach (var subject in response.Data)
                    {
                        worksheet.Cell(row, 1).Value = subject.SubjectCode;
                        worksheet.Cell(row, 2).Value = subject.Name;
                        worksheet.Cell(row, 11).Value = subject.IsEnable ? "Yes" : "No";

                        row++;

                    }
                    string fileName = $"SubjectList_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

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

            var response = _subjectRepository.GetSubjects(isEnabled);
            var subjects = response.Data;
            var subjectViewModel = new SubjectViewModel
            {
                SubjectList = subjects,
                SearchView = new SearchViewModel()
            };
            return new ViewAsPdf("_SubjectListPdf", subjectViewModel)
            {
                FileName = $"SubjectList_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking --print-media-type --viewport-size 1280x1024"
            };
        }
    }
}
