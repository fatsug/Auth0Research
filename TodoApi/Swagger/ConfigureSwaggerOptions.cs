using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TodoApi.Swagger;

public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
{
    private IConfiguration _configuration;

    public ConfigureSwaggerOptions(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    // public void Configure(SwaggerGenOptions options)
    // {
    //     options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //     {
    //         In = ParameterLocation.Header,
    //         Description = "Please provide a valid token",
    //         Name = "Authorization",
    //         Type = SecuritySchemeType.Http,
    //         BearerFormat = "JWT",
    //         Scheme = "Bearer"
    //     });
    //
    //     options.AddSecurityRequirement(new OpenApiSecurityRequirement
    //     {
    //         {
    //             new OpenApiSecurityScheme
    //             {
    //                 Reference = new OpenApiReference
    //                 {
    //                     Type = ReferenceType.SecurityScheme,
    //                     Id = "Bearer"
    //                 }
    //             },
    //             Array.Empty<string>()
    //         }
    //     });
    // }
    public void Configure(SwaggerGenOptions options)
    {
        options.ResolveConflictingActions(x => x.First());
        options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            BearerFormat = "JWT",
            Flows = new OpenApiOAuthFlows
            {
                Implicit = new OpenApiOAuthFlow
                {
                    TokenUrl = new Uri($"https://{_configuration["Auth0:Domain"]}/oauth/token"),
                    AuthorizationUrl =
                        new Uri(
                            $"https://{_configuration["Auth0:Domain"]}/authorize?audience={_configuration["Auth0:Audience"]}"),
                    Scopes = new Dictionary<string, string>
                    {
                        {"openid", "OpenId"},
                    }
                }
            }
        });
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "oauth2"}
                },
                new[] {"openid"}
            }
        });

        // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        // options.IncludeXmlComments(xmlPath);
    }
}