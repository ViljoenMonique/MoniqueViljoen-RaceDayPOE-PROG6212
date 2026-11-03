using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ========== LIST ALL EVENTS ==========
        public async Task<IActionResult> Index()
        {
            var events = await _apiService.GetAllEventsAsync();
            return View(events);
        }

        // ========== EVENT DETAILS (updated with images) ==========
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _apiService.GetEventByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            var categories = await _apiService.GetCategoriesByEventAsync(id) ?? new List<CategoryDto>();
            var images = await _apiService.GetEventImagesAsync(id) ?? new List<EventImageDto>();

            ViewBag.Categories = categories;
            ViewBag.Images = images;
            ViewBag.IsParticipant = HttpContext.Session.GetString("Role") == "Participant";

            return View(eventItem);
        }

        // ========== ENTER EVENT (POST) ==========
        [HttpPost]
        public async Task<IActionResult> EnterEvent(int eventId, int categoryId)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Participant")
                return RedirectToAction("Login", "Account");

            var request = new CreateEnrolmentRequest
            {
                EventId = eventId,
                CategoryId = categoryId
            };

            var result = await _apiService.CreateEnrolmentAsync(request);
            if (result == null)
            {
                TempData["Error"] = "Failed to enter event. You may already be enrolled or the category is invalid.";
                return RedirectToAction("Details", new { id = eventId });
            }

            TempData["Success"] = "Successfully enrolled!";
            return RedirectToAction("MyEnrolments", "Enrolments");
        }

        // ========== CREATE EVENT (GET) ==========
        [HttpGet]
        public IActionResult Create()
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // ========== CREATE EVENT (POST) ==========
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventRequest model)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
            {
                return RedirectToAction("Login", "Account");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _apiService.CreateEventAsync(model);
            if (result == null)
            {
                ModelState.AddModelError("", "Failed to create event. Please try again.");
                return View(model);
            }

            return RedirectToAction("Index", "Organiser");
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            var eventItem = await _apiService.GetEventByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            var model = new UpdateEventRequest
            {
                EventName = eventItem.EventName,
                Description = eventItem.Description,
                EventDate = eventItem.EventDate,
                Location = eventItem.Location,
                DistanceKm = eventItem.DistanceKm,
                EventType = eventItem.EventType,
                MaxParticipants = eventItem.MaxParticipants,
                Status = eventItem.Status
            };

            ViewBag.EventId = id;
            return View(model);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateEventRequest request)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                ViewBag.EventId = id;
                return View(request);
            }

            var result = await _apiService.UpdateEventAsync(id, request);
            if (result == null)
            {
                TempData["Error"] = "Failed to update event. Please try again.";
                ViewBag.EventId = id;
                return View(request);
            }

            TempData["Success"] = "Event updated successfully.";
            return RedirectToAction("Index", "Organiser");
        }

        // POST: Events/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            var success = await _apiService.DeleteEventAsync(id);
            if (success)
                TempData["Success"] = "Event deleted successfully.";
            else
                TempData["Error"] = "Failed to delete event.";

            return RedirectToAction("Index", "Organiser");
        }

        // ========== UPLOAD IMAGE ==========
        [HttpPost]
        public async Task<IActionResult> UploadImage(int eventId, IFormFile imageFile, string? caption)
        {
            var role = HttpContext.Session.GetString("Role");
            if (role != "Organiser")
                return RedirectToAction("Login", "Account");

            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Please select an image.";
                return RedirectToAction("Details", new { id = eventId });
            }

            var result = await _apiService.UploadEventImageAsync(eventId, imageFile, caption);
            if (result == null)
            {
                TempData["Error"] = "Failed to upload image.";
                return RedirectToAction("Details", new { id = eventId });
            }

            TempData["Success"] = "Image uploaded successfully.";
            return RedirectToAction("Details", new { id = eventId });
        }
    }
}