using Microsoft.AspNetCore.Mvc;

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
    }
}
