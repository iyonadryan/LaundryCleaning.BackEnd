using LaundryCleaning.Service.Auth.CustomModels;
using Microsoft.AspNetCore.Identity.Data;

namespace LaundryCleaning.Service.Auth.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthLoginCustomModel> AuthLogin(LoginRequest request, CancellationToken cancellationToken);
    }
}
