using Microsoft.AspNetCore.Mvc;
using Vts_Ecommerce.Helpers;

namespace Vts_Ecommerce.Controllers
{
    public class LocalizationController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage([FromBody] LanguageRequest request)
        {
            if (string.IsNullOrEmpty(request?.Language)) 
                request = new LanguageRequest { Language = "en" };
            
            // Validate language
            var language = request.Language.ToLower();
            if (language != "en" && language != "ar")
                language = "en";

            LocalizationService.SetLanguage(HttpContext, language);
            LocalizationService.SetCulture(language);

            return Ok(new { success = true, language });
        }

        [HttpGet]
        public IActionResult GetCurrentLanguage()
        {
            var lang = LocalizationService.GetCurrentLanguage(HttpContext);
            return Ok(new { language = lang, isRtl = LocalizationService.IsRtl(lang) });
        }
    }

    public class LanguageRequest
    {
        public string Language { get; set; }
    }
}
