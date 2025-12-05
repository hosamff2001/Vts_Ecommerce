using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Vts_Ecommerce.Helpers;

namespace Vts_Ecommerce.Controllers
{
    /// <summary>
    /// Base controller with authorization enforcement
    /// All controllers (except Account) must inherit from this to enforce login requirement
    /// </summary>
    public class AuthorizedController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if user is logged in and session is valid
            if (!SessionManager.IsSessionValid(HttpContext))
            {
                // Redirect to login
                context.Result = RedirectToAction("Login", "Account");
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
