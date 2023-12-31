using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TodoApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
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


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", (ClaimsPrincipal user) =>
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi()
    .RequireAuthorization("weather:read-write");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int) (TemperatureC / 0.5556);
}