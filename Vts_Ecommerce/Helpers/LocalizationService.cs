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

        private const string LanguageCookieKey = "AppLanguage";
        private const string DefaultLanguage = "en";

        /// <summary>
        /// Get the current language from cookie, default to "en"
        /// </summary>
        public static string GetCurrentLanguage(HttpContext httpContext)
        {
            if (httpContext?.Request.Cookies.TryGetValue(LanguageCookieKey, out var lang) == true)
            {
                if (lang == "en" || lang == "ar")
                    return lang;
            }
            return DefaultLanguage;
        }

        /// <summary>
        /// Set language cookie (expires in 1 year) and persist it
        /// </summary>
        public static void SetLanguage(HttpContext httpContext, string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = DefaultLanguage;

            // Normalize and validate language code
            language = language.ToLower().Trim();
            if (language != "en" && language != "ar")
                language = DefaultLanguage;

            var cookieOptions = new CookieOptions 
            { 
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                HttpOnly = false,  // Allow access from JavaScript
                IsEssential = true,
                SameSite = SameSiteMode.Lax,  // Allow cross-site POST
                Secure = false  // Works with both HTTP and HTTPS
            };
            httpContext?.Response.Cookies.Append(LanguageCookieKey, language, cookieOptions);
            
            // Also set culture immediately
            SetCulture(language);
        }

        /// <summary>
        /// Get localized string from resource file
        /// </summary>
        public static string GetString(string key, string? language = null)
        {
            try
            {
                var lang = language ?? GetDefaultLanguage();
                var cultureInfo = new CultureInfo(lang);
                var value = ResManager.GetString(key, cultureInfo);
                return value ?? key;
            }
            catch
            {
                return key;
            }
        }

        /// <summary>
        /// Set current thread culture
        /// </summary>
        public static void SetCulture(string language)
        {
            if (string.IsNullOrWhiteSpace(language))
                language = DefaultLanguage;

            try
            {
                var cultureInfo = new CultureInfo(language);
                CultureInfo.CurrentCulture = cultureInfo;
                CultureInfo.CurrentUICulture = cultureInfo;
            }
            catch
            {
                var defaultCulture = new CultureInfo(DefaultLanguage);
                CultureInfo.CurrentCulture = defaultCulture;
                CultureInfo.CurrentUICulture = defaultCulture;
            }
        }

        /// <summary>
        /// Check if language is RTL (Arabic)
        /// </summary>
        public static bool IsRtl(string language)
        {
            return language?.Equals("ar", StringComparison.OrdinalIgnoreCase) == true;
        }

        private static string GetDefaultLanguage()
        {
            return DefaultLanguage;
        }
    }
}
