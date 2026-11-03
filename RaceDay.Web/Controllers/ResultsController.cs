using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApiService _apiService;

        public ResultsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Participant: My Results
        public async Task<IActionResult> MyResults()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant")
                return RedirectToAction("Login", "Account");

            var results = await _apiService.GetMyResultsAsync();
            return View(results);
        }

        // Organiser: Capture Result - GET
        [HttpGet]
        public IActionResult Create(int enrolmentId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            ViewBag.EnrolmentId = enrolmentId;
            return View();
        }

        // Organiser: Capture Result - POST
        [HttpPost]
        public async Task<IActionResult> Create(int EnrolmentId, string FinishTime, int Position, string Status, string Notes)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            var request = new CreateResultRequest
            {
                EnrolmentId = EnrolmentId,
                FinishTime = TimeSpan.TryParse(FinishTime, out var time) ? time : null,
                Position = Position,
                Status = string.IsNullOrWhiteSpace(Status) ? "Finished" : Status,
                Notes = Notes
            };

            var result = await _apiService.CreateResultAsync(request);

            if (result == null)
            {
                ViewBag.Error = "Failed to save result. Please make sure you are logged in as Organiser and try again.";
                ViewBag.EnrolmentId = EnrolmentId;
                return View();
            }

            return RedirectToAction("Index", "Organiser");
        }
    }
}
