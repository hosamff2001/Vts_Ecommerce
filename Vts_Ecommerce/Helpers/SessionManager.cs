using System;
using Microsoft.AspNetCore.Http;
using Vts_Ecommerce.DAL.Repositories;
using Vts_Ecommerce.Models;

namespace Vts_Ecommerce.Helpers
{
    /// <summary>
    /// Session Manager for handling user sessions
    /// Uses ASP.NET Core `ISession` via HttpContext
    /// </summary>
    public static class SessionManager
    {
        private const string SessionUserIdKey = "UserId";
        private const string SessionUsernameKey = "Username";
        private const string SessionIdKey = "SessionId";

        // Create a session entry: writes to HttpContext.Session and persists UserSession record
        public static void CreateSession(HttpContext httpContext, int userId, string username, string deviceInfo = null)
        {
            if (httpContext == null)
                throw new ArgumentNullException(nameof(httpContext));

            var sessionId = Guid.NewGuid().ToString();
            httpContext.Session.SetInt32(SessionUserIdKey, userId);
            httpContext.Session.SetString(SessionUsernameKey, username ?? string.Empty);
            httpContext.Session.SetString(SessionIdKey, sessionId);

            // enforce single-device login: deactivate any existing sessions
            var repo = new UserSessionRepository();
            var existing = repo.GetActiveByUserId(userId);
            if (existing != null)
            {
                repo.DeactivateBySessionId(existing.SessionId);
            }

            var userSession = new UserSession
            {
                UserId = userId,
                SessionId = sessionId,
                DeviceInfo = deviceInfo,
                LoginTime = DateTime.Now,
                LastActivityTime = DateTime.Now,
                IsActive = true
            };
            repo.Create(userSession);
        }

        // Validate current session by checking DB record
        public static bool IsSessionValid(HttpContext httpContext)
        {
            if (httpContext == null)
                return false;

            var userId = httpContext.Session.GetInt32(SessionUserIdKey);
            var sessionId = httpContext.Session.GetString(SessionIdKey);
            if (!userId.HasValue || string.IsNullOrEmpty(sessionId))
                return false;

            var repo = new UserSessionRepository();
            var session = repo.GetBySessionId(sessionId);
            if (session == null || !session.IsActive)
                return false;

            // update last activity
            repo.UpdateLastActivity(sessionId, DateTime.Now);
            return true;
        }

        public static void ClearSession(HttpContext httpContext)
        {
            if (httpContext == null)
                return;

            var sessionId = httpContext.Session.GetString(SessionIdKey);
            if (!string.IsNullOrEmpty(sessionId))
            {
                var repo = new UserSessionRepository();
                repo.DeactivateBySessionId(sessionId);
            }

            httpContext.Session.Remove(SessionUserIdKey);
            httpContext.Session.Remove(SessionUsernameKey);
            httpContext.Session.Remove(SessionIdKey);
        }

        public static int? GetCurrentUserId(HttpContext httpContext)
        {
            return httpContext?.Session.GetInt32(SessionUserIdKey);
        }

        public static string GetCurrentUsername(HttpContext httpContext)
        {
            return httpContext?.Session.GetString(SessionUsernameKey);
        }
    }
}

