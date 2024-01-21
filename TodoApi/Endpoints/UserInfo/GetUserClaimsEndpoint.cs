using FastEndpoints;

namespace TodoApi.Endpoints.UserInfo;

public class GetUserClaimsEndpoint : EndpointWithoutRequest<IEnumerable<UserClaim>>
{
    public override void Configure()
    {
        Get("/user-claims");
        Policies("ReadProfiles");
        Group<AdministrationEndpointGroup>();
    }

    public override Task HandleAsync(CancellationToken ct)
    {
        var claims = User.Claims.Select(c => new UserClaim(c.Type, c.Value)).ToArray();
        
        Response = claims;
        return Task.CompletedTask;
    }
}

public record UserClaim(string Type, string Value);
