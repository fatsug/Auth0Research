using FastEndpoints;

namespace TodoApi.Endpoints.UserInfo;

public sealed class AdministrationEndpointGroup : Group
{
    public AdministrationEndpointGroup()
    {
        Configure("User", ep => //admin is the route prefix for the top level group
        {
            ep.Description(x => x
                .WithTags("User")
            );
        });
    }
}