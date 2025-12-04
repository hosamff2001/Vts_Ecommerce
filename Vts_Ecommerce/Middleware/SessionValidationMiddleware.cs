using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Vts_Ecommerce.Helpers;

namespace Vts_Ecommerce.Middleware
{
    /// <summary>
    /// Middleware for validating user session on each request
    /// Forces logout if session expired or invalid
    /// Prevents multi-device login
    /// </summary>
    public class SessionValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public SessionValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Skip validation for login, logout, and public paths
            if (path == null || 
                path.StartsWith("/account/login") ||
                path.StartsWith("/account/logout") ||
                path.StartsWith("/css/") ||
                path.StartsWith("/js/") ||
                path.StartsWith("/lib/") ||
                path == "/" ||
                path.StartsWith("/home/"))
            {
                await _next(context);
                return;
            }

            var userId = context.Session.GetInt32("UserId");
            var sessionId = context.Session.GetString("SessionId");

            // If user is logged in, validate session
            if (userId.HasValue && !string.IsNullOrEmpty(sessionId))
            {
                if (!SessionManager.IsSessionValid(context))
                {
                    // Session invalid or expired - logout
                    SessionManager.ClearSession(context);
                    context.Response.Redirect("/Account/Login?expired=true");
                    return;
                }
            }

            await _next(context);
        }
    }
}
