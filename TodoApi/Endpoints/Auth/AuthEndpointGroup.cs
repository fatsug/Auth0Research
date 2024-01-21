using FastEndpoints;

namespace TodoApi.Endpoints.Auth;

public sealed class AuthEndpointGroup : Group
{
    public AuthEndpointGroup()
    {
        Configure("Auth", ep => //admin is the route prefix for the top level group
        {
            ep.Description(x => x
                .WithTags("Auth")
            );
        });
    }
}