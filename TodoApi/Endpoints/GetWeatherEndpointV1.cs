using System.Security.Claims;
using FastEndpoints;

namespace TodoApi.Endpoints;

public class GetWeatherEndpointV1 : EndpointWithoutRequest<WeatherForecast>
{
    public override void Configure()
    {
        Get("/weatherforecast");
        AllowAnonymous();
        Version(1);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .FirstOrDefault();

        if (forecast is null) return;
        await SendAsync(forecast, cancellation: ct);
    }
}