using Auth0.AspNetCore.Authentication;
using FastEndpoints;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace TodoApi.Endpoints.Auth;

public class LogoutEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/logout");
        Group<AuthEndpointGroup>();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        const string returnUrl = "/";

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, new AuthenticationProperties
        {
            RedirectUri = returnUrl
        });
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }
}