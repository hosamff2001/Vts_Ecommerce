using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Models;
using Vts_Ecommerce.Helpers;

namespace Vts_Ecommerce.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserRepository _userRepo = new UserRepository();

        public IActionResult Login()
        {
            // If already logged in and session valid, redirect home
            if (SessionManager.IsSessionValid(HttpContext))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ViewBag.Error = "Username and password are required.";
                return View();
            }

            var user = _userRepo.GetByUsername(username);
            if (user == null || !user.IsActive)
            {
                ViewBag.Error = "Invalid username or password.";
                return View();
            }

            try
            {
                // stored password is AES-encrypted; decrypt and compare
                var decrypted = EncryptionService.Decrypt(user.Password);
                if (decrypted != password)
                {
                    ViewBag.Error = "Invalid username or password.";
                    return View();
                }

                // Check for existing active session
                var sessionRepo = new UserSessionRepository();
                var activeSession = sessionRepo.GetActiveByUserId(user.Id);

                // If active session exists and was active within last 20 minutes, block login
                if (activeSession != null && activeSession.LastActivityTime > DateTime.Now.AddMinutes(-20))
                {
                    ViewBag.Error = "User is already active in another session.";
                    return View();
                }

                // Create session and user session record
                var deviceInfo = Request.Headers["User-Agent"].ToString();
                SessionManager.CreateSession(HttpContext, user.Id, user.Username, deviceInfo);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred during login.";
                return View();
            }
        }

        public IActionResult Logout()
        {
            SessionManager.ClearSession(HttpContext);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public IActionResult Keepalive()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (!userId.HasValue)
                return Unauthorized();

            // Keepalive is handled by SessionManager.IsSessionValid() in the middleware
            // which updates LastActivityTime
            return Ok(new { success = true });
        }
    }
}
