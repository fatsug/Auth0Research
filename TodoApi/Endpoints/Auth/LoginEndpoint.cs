using Auth0.AspNetCore.Authentication;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;

namespace TodoApi.Endpoints.Auth;

public class LoginEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/login");
        Group<AuthEndpointGroup>();
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        const string returnUrl = "/user-id";

        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(returnUrl)
            .Build();

        await HttpContext.ChallengeAsync(
            Auth0Constants.AuthenticationScheme,
            authenticationProperties
        );
    }
}