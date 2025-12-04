using Microsoft.AspNetCore.Mvc;
using Vts_Ecommerce.Helpers;

namespace Vts_Ecommerce.Controllers
{
    public class LocalizationController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string language)
        {
            if (string.IsNullOrEmpty(language)) language = "en";
            
            LocalizationService.SetLanguage(HttpContext, language);
            LocalizationService.SetCulture(language);

            return Ok(new { success = true, language });
        }

        public IActionResult GetCurrentLanguage()
        {
            var lang = LocalizationService.GetCurrentLanguage(HttpContext);
            return Ok(new { language = lang, isRtl = LocalizationService.IsRtl(lang) });
        }
    }
}
