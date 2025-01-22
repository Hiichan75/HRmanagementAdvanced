using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

public class LanguageController : Controller
{
    [HttpPost]
    public IActionResult SetLanguage(string culture, string returnUrl)
    {
        // Log the current culture cookie value (useful for debugging)
        var cultureCookie = Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
        Console.WriteLine($"Current Culture Cookie Value: {cultureCookie}");

        if (!string.IsNullOrEmpty(culture))
        {
            // Append the new culture to the response cookies
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    HttpOnly = true,
                    SameSite = SameSiteMode.Lax
                }
            );
        }

        // Redirect the user to the provided return URL or the homepage
        return LocalRedirect(returnUrl ?? "/");
    }
}
