using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Http;

namespace Vts_Ecommerce.Helpers
{
    /// <summary>
    /// Localization service for multi-language support (English and Arabic)
    /// </summary>
    public static class LocalizationService
    {
        private static readonly ResourceManager ResManager = 
            new ResourceManager("Vts_Ecommerce.Resources.Shared", Assembly.GetExecutingAssembly());

        private const string LanguageCookieKey = "Language";
        private const string DefaultLanguage = "en";

        // Get the current language from cookie, default to "en"
        public static string GetCurrentLanguage(HttpContext httpContext)
        {
            if (httpContext?.Request.Cookies.TryGetValue(LanguageCookieKey, out var lang) == true)
            {
                return lang;
            }
            return DefaultLanguage;
        }

        // Set language cookie and update culture
        public static void SetLanguage(HttpContext httpContext, string language)
        {
            var options = new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) };
            httpContext?.Response.Cookies.Append(LanguageCookieKey, language, options);
        }

        // Get localized string from resource file
        public static string GetString(string key, string language = null)
        {
            try
            {
                var cultureInfo = new CultureInfo(language ?? DefaultLanguage);
                var value = ResManager.GetString(key, cultureInfo);
                return value ?? key;
            }
            catch
            {
                return key;
            }
        }

        // Set current thread culture
        public static void SetCulture(string language)
        {
            var cultureInfo = new CultureInfo(language ?? DefaultLanguage);
            CultureInfo.CurrentCulture = cultureInfo;
            CultureInfo.CurrentUICulture = cultureInfo;
        }

        // Check if language is RTL (Arabic)
        public static bool IsRtl(string language)
        {
            return language?.StartsWith("ar") == true;
        }
    }
}
