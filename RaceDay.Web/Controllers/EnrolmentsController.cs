using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly ApiService _apiService;

        public EnrolmentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Participant: My Enrolments
        public async Task<IActionResult> MyEnrolments()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant")
                return RedirectToAction("Login", "Account");

            var enrolments = await _apiService.GetMyEnrolmentsAsync();
            return View(enrolments);
        }

        // Organiser: View enrolments for an event
        public async Task<IActionResult> EventEnrolments(int eventId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            var enrolments = await _apiService.GetEnrolmentsByEventAsync(eventId);
            ViewBag.EventId = eventId;
            return View(enrolments);
        }

        // Participant: Enrol in an event
        [HttpPost]
        public async Task<IActionResult> Enrol(CreateEnrolmentRequest model)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant")
                return RedirectToAction("Login", "Account");

            var result = await _apiService.CreateEnrolmentAsync(model);
            if (result == null)
            {
                TempData["Error"] = "Failed to enrol. You may already be enrolled.";
            }
            else
            {
                TempData["Success"] = "Successfully enrolled!";
            }

            return RedirectToAction("Details", "Events", new { id = model.EventId });
        }
    }
}
