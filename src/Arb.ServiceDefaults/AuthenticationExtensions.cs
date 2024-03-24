using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Arb.ServiceDefaults;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddDefaultAuthentication(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration; 
        
        // {
        //   "Microservice": {
        //     "IdentityUrl": "http://identity-api",
        //     "ClientId": "ArbTech-crm-party-model"
        //    }
        // }

        var identitySection = configuration.GetSection("Microservice");

        if (!identitySection.Exists())
        {
            // No identity section, so no authentication
            return services;
        }

        // prevent from mapping "sub" claim to nameidentifier.
        JsonWebTokenHandler.DefaultInboundClaimTypeMap.Remove("sub");

        var audience = identitySection.GetRequiredValue("ClientId");
 
        services.AddAuthentication("Bearer").AddJwtBearer(options =>
        {
            var identityUrl = identitySection.GetRequiredValue("IdentityUrl");
            
            options.Authority = identityUrl;
            options.RequireHttpsMetadata = false;
            options.Audience = audience;
            options.TokenValidationParameters.ValidIssuer = "http://identity-api";
            options.TokenValidationParameters.ValidAudience = audience;
            options.TokenValidationParameters.ValidateAudience = false;
            
            X509Certificate2 cert = new("rsa.pfx", "rut plausible endorse worsening");
            SecurityKey key = new X509SecurityKey(cert);
            options.TokenValidationParameters.IssuerSigningKey = key;
        });
        
        // services.AddAuthorization();
        services.AddAuthorizationBuilder()
            .AddPolicy("ApiScope", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireClaim("scope", audience);
            });

        return services;
    }
}
