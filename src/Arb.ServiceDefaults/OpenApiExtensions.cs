using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Arb.ServiceDefaults;

public static class OpenApiExtensions
{
    public static IApplicationBuilder UseDefaultOpenApi(this WebApplication app)
    {
        var configuration = app.Configuration;
        var openApiSection = configuration.GetSection("OpenApi");

        if (!openApiSection.Exists())
        {
            return app;
        }
        var microserviceSection = configuration.GetSection("Microservice");
        
        app.UseSwagger();
        app.UseSwaggerUI(setup =>
        {
            /// {
            //   "Microservice": {
            //     "ClientId": "ArbTech-crm-party-model"
            //     "ApplicationName": "Party Model"
            //    }
            /// }

            
            setup.SwaggerEndpoint("v1/swagger.json", microserviceSection.GetRequiredValue("ApplicationName"));
            // setup.RoutePrefix = $"{microserviceSection.GetRequiredValue("ShortRoute")}/swagger";
            
            setup.OAuthClientId($"{microserviceSection.GetRequiredValue("ClientId")}-swaggerui");
            setup.OAuthAppName(microserviceSection.GetRequiredValue("ApplicationName"));
        });
        
        
        app.UseReDoc(options =>
        {
            options.DocumentTitle = $"{microserviceSection.GetRequiredValue("ApplicationName")} Documentation"; 
        });

        // Add a redirect from the root of the app to the swagger endpoint
        app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

        return app;
    }

    public static IHostApplicationBuilder AddDefaultOpenApi(this IHostApplicationBuilder builder)
    {
        var services = builder.Services;
        var configuration = builder.Configuration;

        var openApi = configuration.GetSection("OpenApi");

        if (!openApi.Exists())
        {
            return builder;
        }

        services.AddGrpcSwagger();
        // services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            /// {
            ///   "OpenApi": {
            ///     "Document": {
            ///         "Title": ..
            ///         "Version": ..
            ///         "Description": ..
            ///     },
            ///     "Contact": {
            ///         "ProjectName": ...
            ///         "Email": ...
            ///         "Url": ...
            ///         "LogoUrl": ...
            ///     }
            ///   }
            /// }
            var document = openApi.GetRequiredSection("Document");

            var version = document.GetRequiredValue("Version");

            var contact = openApi.GetRequiredSection("Contact");

            options.SwaggerDoc(version,
                new OpenApiInfo
                {
                    Title = document.GetRequiredValue("Title"),
                    Version = version,
                    Description = document.GetRequiredValue("Description"),
                    Contact = new OpenApiContact
                    {
                        Name = contact.GetRequiredValue("ProjectName"),
                        Email = contact.GetRequiredValue("Email"),
                        Url = new Uri(contact.GetRequiredValue("Url"))
                    },
                    Extensions = new Dictionary<string, IOpenApiExtension>
                    {
                        {
                            "x-logo",
                            new OpenApiObject
                            {
                                { "url", new OpenApiString(contact.GetRequiredValue("LogoUrl")) },
                                { "altText", new OpenApiString(contact.GetRequiredValue("ProjectName")) }
                            }
                        }
                    }
                });

            var identitySection = configuration.GetSection("Microservice");

            if (!identitySection.Exists())
            {
                // No identity section, so no authentication open api definition
                return;
            }

            // {
            //   "Microservice": {
            //     "IdentityUrl": "http://identity",
            //     "ClientId": "ArbTech-crm-party-model"
            //     "ApplicationName": "Party Model"
            //    }
            // }

            var identityUrlExternal = identitySection.GetRequiredValue("IdentityUrl");
            var scopes = new Dictionary<string, string>()
            {
                { identitySection.GetRequiredValue("ClientId"), identitySection.GetRequiredValue("ApplicationName") }
            };

            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows()
                {
                    // TODO: Change this to use Authorization Code flow with PKCE
                    Implicit = new OpenApiOAuthFlow()
                    {
                        AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                        TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                        Scopes = scopes,
                    }
                }
            });

            options.OperationFilter<AuthorizeCheckOperationFilter>([scopes.Keys.ToArray()]);
        });

        return builder;
    }

    private sealed class AuthorizeCheckOperationFilter(string[] scopes) : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

            if (!metadata.OfType<IAuthorizeData>().Any())
            {
                return;
            }

            operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

            var oAuthScheme = new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
            };

            operation.Security = new List<OpenApiSecurityRequirement> { new() { [oAuthScheme] = scopes } };
        }
    }
}
