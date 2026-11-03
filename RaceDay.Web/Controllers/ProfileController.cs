using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiService _apiService;

        public ProfileController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var profile = await _apiService.GetMyProfileAsync();
            if (profile == null)
                return RedirectToAction("Login", "Account");

            return View(profile);
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var profile = await _apiService.GetMyProfileAsync();
            if (profile == null)
                return RedirectToAction("Login", "Account");

            var model = new UpdateProfileRequest
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Phone = profile.Phone
            };

            ViewBag.ProfileImageUrl = profile.ProfileImageUrl;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var result = await _apiService.UpdateProfileAsync(request);
            if (result == null)
            {
                ModelState.AddModelError("", "Failed to update profile.");
                return View(request);
            }

            TempData["Success"] = "Profile updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                TempData["Error"] = "Please select an image.";
                return RedirectToAction("Edit");
            }

            var imageUrl = await _apiService.UploadProfileImageAsync(imageFile);
            if (imageUrl == null)
            {
                TempData["Error"] = "Failed to upload image.";
                return RedirectToAction("Edit");
            }

            TempData["Success"] = "Profile image updated.";
            return RedirectToAction("Index");
        }
    }
}
