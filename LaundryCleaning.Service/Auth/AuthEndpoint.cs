using LaundryCleaning.Service.Auth.Services.Interfaces;
using Microsoft.AspNetCore.Identity.Data;

namespace LaundryCleaning.Service.Auth
{
    public static class AuthEndpoint
    {
        public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapPost("/login", async (
                LoginRequest request,
                IAuthService authService,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var response = await authService.AuthLogin(request, cancellationToken);
                    return Results.Ok(response);
                }
                catch (UnauthorizedAccessException)
                {
                    return Results.Unauthorized();
                }
            });

            return endpoints;
        }
    }

}
