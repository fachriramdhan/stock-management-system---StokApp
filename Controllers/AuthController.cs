using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Models;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Login
        public IActionResult Login()
        {
            if (SessionHelper.IsLoggedIn(HttpContext))
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Username dan password harus diisi!";
                return View();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                ViewBag.Error = "Username atau password salah!";
                return View();
            }

            // Set session
            SessionHelper.SetSession(HttpContext, "UserId", user.Id.ToString());
            SessionHelper.SetSession(HttpContext, "Username", user.Username);
            SessionHelper.SetSession(HttpContext, "UserRole", user.Role);

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: Logout
        public IActionResult Logout()
        {
            SessionHelper.ClearSession(HttpContext);
            return RedirectToAction("Login");
        }

        // GET: Access Denied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}