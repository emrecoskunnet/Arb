using System.Globalization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using ArbTech.Infrastructure.Constants;

namespace ArbTech.Infrastructure.Localization;

public static class LocalizationExtensions
{
    public static void AddCustomLocalization(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddSingleton<LocalizerMiddleware>();
        services.AddTransient<IAppLocalizer, JsonStringLocalizer>();
        services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
    }

    public static void UseCustomLocalization(this IApplicationBuilder application)
    {
        RequestLocalizationOptions options = new()
        {
            DefaultRequestCulture = new RequestCulture(new CultureInfo(DefaultsConstants.DefaultCulture))
        };

        application.UseRequestLocalization(options);
        application.UseMiddleware<LocalizerMiddleware>();
    }
}
