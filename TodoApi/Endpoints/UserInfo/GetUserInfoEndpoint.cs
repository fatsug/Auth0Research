// using FastEndpoints;
//
// namespace TodoApi.Endpoints.UserInfo;
//
// public class GetUserInfoEndpoint(IAuthenticationApiClient client)
//     : EndpointWithoutRequest<Auth0.AuthenticationApi.Models.UserInfo>
// {
//     public override void Configure()
//     {
//         Get("/user-info");
//         Policies("job:read");
//         Group<AdministrationEndpointGroup>();
//     }
//
//     public override async Task HandleAsync(CancellationToken ct)
//     {
//         // Retrieve the access_token claim which we saved in the OnTokenValidated event
//         var accessToken = User.Claims.FirstOrDefault(c => c.Type == "access_token")?.Value;
//
//         // If we have an access_token, then retrieve the user's information
//         if (!string.IsNullOrEmpty(accessToken))
//         {
//             Response = await client.GetUserInfoAsync(accessToken, ct);
//         }
//
//         await Task.CompletedTask;
//     }
// }