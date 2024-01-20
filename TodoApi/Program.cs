using System.Security.Claims;
using System.Text;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddFastEndpoints();
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .SwaggerDocument(o =>
    {
        o.DocumentSettings = s =>
        {
            s.DocumentName = "Initial Release";
            s.Title = "my api";
            s.Version = "v0";
        };
    })
    .SwaggerDocument(o =>
    {
        o.MaxEndpointVersion = 1;
        o.DocumentSettings = s =>
        {
            s.DocumentName = "Release 1.0";
            s.Title = "my api";
            s.Version = "v1.0";
        };
    })
    .SwaggerDocument(o =>
    {
        o.MaxEndpointVersion = 2;
        o.DocumentSettings = s =>
        {
            s.DocumentName = "Release 2.0";
            s.Title = "my api";
            s.Version = "v2.0";
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Weather API",
        Description = "Web APIs for Weather forecasts",
        Version = "v1"
    });

    // c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    // {
    //     Type = SecuritySchemeType.OAuth2,
    //     Flows = new OpenApiOAuthFlows
    //     {
    //         Implicit = new OpenApiOAuthFlow
    //         {
    //             Scopes = new Dictionary<string, string>
    //             {
    //                 {"openid", "Open Id"}
    //             },
    //             AuthorizationUrl =
    //                 new Uri(
    //                     $"https://{builder.Configuration["Auth0:domain"]}/authorize?audience={builder.Configuration["Auth0:Audience"]}")
    //         }
    //     }
    // });
});

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
//     {
//         c.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
//         c.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidAudience = builder.Configuration["Auth0:Audience"],
//             ValidIssuer = builder.Configuration["Auth0:Domain"]
//         };
//     });

// generic
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidAudience = builder.Configuration["Auth0:Audience"],
        ValidIssuer = builder.Configuration["Auth0:Domain"],
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth0:Key"]!))
    };
});

builder.Services.AddAuthorization(o =>
{
    // o.AddPolicy("weather:read-write", p => p
    //     .RequireAuthenticatedUser()
    //     .RequireClaim("scope", "weather:read-write"));
    
    o.AddPolicy("weather:read-write", p => p
        .RequireAuthenticatedUser()
        .RequireClaim("scope", "openid"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
}

app.MapSwagger();
app.UseSwaggerUI(settings =>
{
    settings.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1.0");
    settings.OAuthClientId(builder.Configuration["Auth0:ClientId"]);
    settings.OAuthClientSecret(builder.Configuration["Auth0:ClientSecret"]);
    settings.OAuthUsePkce();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
});
app.Run();

