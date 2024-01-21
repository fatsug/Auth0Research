using Auth0.AspNetCore.Authentication;
using FastEndpoints;
using FastEndpoints.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument();
// builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = builder.Configuration["Auth0:Domain"];
    options.ClientId = builder.Configuration["Auth0:ClientId"];
});


builder.Services.AddAuthorization(o =>
{
    o.AddPolicy("WriteJobs", p => p
        .RequireAuthenticatedUser()
        .RequireClaim("scope", "openid")
        .RequireClaim("permissions", "write:jobs"));
    o.AddPolicy("ReadJobs", p => p
        .RequireAuthenticatedUser()
        .RequireClaim("scope", "openid")
        .RequireClaim("permissions", "read:jobs"));
    o.AddPolicy("ReadProfiles", p => p
        .RequireAuthenticatedUser()
        .RequireClaim("scope", "openid")
        .RequireClaim("permissions", "read:profiles"));
    o.AddPolicy("WriteProfiles", p => p
        .RequireAuthenticatedUser()
        .RequireClaim("scope", "openid")
        .RequireClaim("permissions", "write:profiles"));
    
    o.AddPolicy("ReadWeatherForecasts", p => p
        .RequireAuthenticatedUser()
        .RequireClaim("scope", "openid")
        .RequireClaim("permissions", "weather:read-write"));
});

const string appName = "Auth0";
builder.Services.SwaggerDocument(o =>
{
    // var secuityScheme = new NSwag.OpenApiSecurityScheme{};

    o.DocumentSettings = s =>
    {
        s.DocumentName = "vNext";
        s.Title = appName + " - Rolling Release";
        s.Version = "vNext";
    };
    o.ShortSchemaNames = true;
});

const int version = 2;
builder.Services.SwaggerDocument(o =>
    {
        o.MaxEndpointVersion = version;
        o.DocumentSettings = s =>
        {
            s.DocumentName = $"v{version}";
            s.Title = appName + " - LTS Release";
            s.Version = $"v{version}";
        };
        o.ShortSchemaNames = true;
    });


var app = builder.Build();

app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
    c.Versioning.PrependToRoute = true;
});
app.UseSwaggerGen();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.Run();

