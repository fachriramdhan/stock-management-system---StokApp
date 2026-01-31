using Microsoft.AspNetCore.Http;

namespace StokApp.Helpers
{
    public static class SessionHelper
    {
        public static void SetSession(HttpContext context, string key, string value)
        {
            context.Session.SetString(key, value);
        }

        public static string? GetSession(HttpContext context, string key)
        {
            return context.Session.GetString(key);
        }

        public static void ClearSession(HttpContext context)
        {
            context.Session.Clear();
        }

        public static bool IsLoggedIn(HttpContext context)
        {
            return !string.IsNullOrEmpty(GetSession(context, "UserId"));
        }

        public static bool IsAdmin(HttpContext context)
        {
            return GetSession(context, "UserRole") == "Admin";
        }
    }
}