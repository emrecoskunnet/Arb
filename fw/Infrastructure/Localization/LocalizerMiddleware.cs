using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace ArbTech.Infrastructure.Localization;

public class LocalizerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        string cultureKey = context.Request.Headers["Accept-Language"].ToString();

        if (!string.IsNullOrEmpty(cultureKey) && DoesCultureExist(cultureKey))
        {
            CultureInfo culture = new(cultureKey);

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        // Await the next request
        await next(context);
    }

    private static bool DoesCultureExist(string cultureName)
    {
        // Return the culture where the culture equals the culture name set
        return CultureInfo.GetCultures(CultureTypes.AllCultures).Any(culture => string.Equals(culture.Name, cultureName,
            StringComparison.CurrentCultureIgnoreCase));
    }
}
