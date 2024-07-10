using Microsoft.AspNetCore.Mvc;
using Rotativa.AspNetCore;
using SMS.BL.Log.Interface;
using SMS.Model.Log;
using SMS.ViewModel.ErrorResponse;
using SMS.ViewModel.Log;
using SMS.ViewModel.RepositoryResponse;
using SMS.ViewModel.StaticData;
using SMS.ViewModel.Student;


namespace SMS_Stored.Controllers
{
    public class LogController : Controller
    {

        private readonly ILogRepository _logRepository;
        public LogController(ILogRepository logRepository)
        {
            _logRepository = logRepository;
            
        }
        public IActionResult Index()
        {
           
            var viewModel = new LogViewModel
            {
                Logs= new List<LogBO>()
                
            };
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult GetLogs(FilterViewModel filterView)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _logRepository.GetLogs(filterView);

                if (response.Success && response.Data != null && response.Data.Any())
                {
                   
                    var logCounts = response.Data
                        .GroupBy(log => log.Level) 
                        .Select(group => new LogLevelCountViewModel
                        {
                            LogLevel = group.Key,
                            Count = group.Count()
                        })
                        .ToList();

                    var viewModel = new LogViewModel
                    {
                        Logs = response.Data,
                        SearchView = new SearchViewModel(),
                        LogCounts = logCounts
                    };

                    return PartialView("_LogList", viewModel);
                }
                else
                {
                    errorResponse.Messages.Add(string.Format(string.Join(", ", response.Messages)));
                    return new JsonResult(errorResponse);
                }
            }
            catch
            {
                errorResponse.Messages.Add(string.Format(StaticMessages.Error_Load_Data, "Logs"));
                return Json(new { success = false, message = errorResponse.Messages });
            }
        }


        public ActionResult SearchLogs(SearchViewModel model)
        {
            var errorResponse = new ErrorResponse();
            try
            {
                var response = _logRepository.SearchLogs(model);
                if (response.Success && response.Data != null && response.Data.Any())
                {
                    var viewModel = new LogViewModel
                    {
                        Logs = response.Data,
                        SearchView = new SearchViewModel()

                    };
                    return PartialView("_LogList", viewModel);
                }
                else
                {
                  
                    return Json(new { success = false, message = response.Messages });
                }

            }
            catch
            {
               
                errorResponse.Messages.Add(string.Format(StaticMessages.Data_Not_Found));
                // errorResponse.Messages.Add(ex.Message); 

                return Json(new { success = false, message = errorResponse.Messages });
            }
        }

        [HttpGet]
        public IActionResult GetLogCountsForChart(FilterViewModel filterView)
        {
            try
            {
                var response = _logRepository.GetLogs(filterView);

                if (response.Success && response.Data != null && response.Data.Any())
                {
                    var logCounts = response.Data
                        .GroupBy(log => log.Level) // Adjust 'Level' to your log level property
                        .Select(group => new
                        {
                            LogLevel = group.Key,
                            Count = group.Count()
                        })
                        .ToList();

                    return Json(logCounts);
                }
                else
                {
                    return Json(new { success = false, message = "No data available" });
                }
            }
            catch
            {
                return Json(new { success = false, message = "Error loading data" });
            }
        }
        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetDailyLogCounts(FilterViewModel filterView)
        {
            try
            {
                var logCounts = _logRepository.GetDailyLogCounts(filterView);

                if (logCounts != null && logCounts.Any())
                {
                    return Json(logCounts);
                }
                else
                {
                    return Json(new { success = false, message = "No data available" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        public IActionResult ExportToPdf(FilterViewModel filterView)
        {
            var response = _logRepository.GetLogs(filterView);
            var logs= response.Data;
            var logViewModel = new LogViewModel
            {
                Logs = logs,
                FilterView=filterView
               
            };
            return new ViewAsPdf("_LogListPdf", logViewModel)
            {
                FileName = $"LogList_{DateTime.Now:yyyyMMdd_HHmmss}.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                CustomSwitches = "--disable-smart-shrinking --print-media-type --viewport-size 1280x1024"
            };
        }

    }
}
