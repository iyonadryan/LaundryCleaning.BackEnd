using LaundryCleaning.Service.Common.Response;
using LaundryCleaning.Service.GraphQL.Roles.Inputs;

namespace LaundryCleaning.Service.GraphQL.Roles.Services.Interfaces
{
    public interface IRoleService
    {
        Task<GlobalSuccessResponseCustomModel> AddRolePermission(AddRolePermissionInput input, CancellationToken cancellationToken);
    }
}
