using Microsoft.AspNetCore.Mvc;
using RaceDay.Contracts;
using RaceDay.Web.Services;

namespace RaceDay.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ===================== REGISTER =====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiService.RegisterAsync(model);

            if (result == null)
            {
                ModelState.AddModelError("", "Registration failed. Email may already exist.");
                return View(model);
            }

            // After successful registration, go to Login
            return RedirectToAction("Login");
        }

        // ===================== LOGIN =====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiService.LoginAsync(model);

            if (result == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(model);
            }

            // Store user info in Session
            HttpContext.Session.SetInt32("UserId", result.UserId);
            HttpContext.Session.SetString("FirstName", result.FirstName ?? "");
            HttpContext.Session.SetString("LastName", result.LastName ?? "");
            HttpContext.Session.SetString("Email", result.Email ?? "");
            HttpContext.Session.SetString("Role", result.Role ?? "");

            // Redirect based on role
            if (result.Role == "Organiser")
                return RedirectToAction("Index", "Organiser");
            else
                return RedirectToAction("Index", "Home");
        }

        // ===================== LOGOUT =====================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
