using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            // Simple protection – only Organisers can access
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            var events = await _apiService.GetAllEventsAsync();
            return View(events);
        }
    }
}
