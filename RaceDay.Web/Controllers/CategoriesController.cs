using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ApiService _apiService;

        public CategoriesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // View categories for a specific event
        public async Task<IActionResult> Index(int eventId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            var categories = await _apiService.GetCategoriesByEventAsync(eventId);
            ViewBag.EventId = eventId;
            return View(categories);
        }

        // GET: Create category form
        [HttpGet]
        public IActionResult Create(int eventId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            ViewBag.EventId = eventId;
            return View();
        }

        // POST: Create category
        [HttpPost]
        public async Task<IActionResult> Create(int eventId, CreateCategoryRequest model)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.EventId = eventId;
                return View(model);
            }

            try
            {
                var result = await _apiService.CreateCategoryAsync(eventId, model);
                if (result == null)
                {
                    ModelState.AddModelError("", "API returned null. Check that the API is running and the BaseAddress in Program.cs is correct.");
                    ViewBag.EventId = eventId;
                    return View(model);
                }

                return RedirectToAction("Index", new { eventId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error calling API: " + ex.Message);
                ViewBag.EventId = eventId;
                return View(model);
            }
        }

        // Delete category
        [HttpPost]
        public async Task<IActionResult> Delete(int id, int eventId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            await _apiService.DeleteCategoryAsync(id);
            return RedirectToAction("Index", new { eventId });
        }
    }
}