using System.Security.Claims;
using FastEndpoints;

namespace TodoApi.Endpoints.UserInfo;

public class GetUserIdEndpoint : EndpointWithoutRequest<string>
{
    public override void Configure()
    {
        Get("/user-id");
        Policies("ReadProfiles");
        Group<AdministrationEndpointGroup>();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        // The user's ID is available in the NameIdentifier claim
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        if (userId != null) Response = userId;
        
        return Task.CompletedTask;
    }
}