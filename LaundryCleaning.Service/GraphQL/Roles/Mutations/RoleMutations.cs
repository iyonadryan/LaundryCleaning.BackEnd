using LaundryCleaning.Service.Common.Constants;
using LaundryCleaning.Service.Common.Response;
using LaundryCleaning.Service.GraphQL.Roles.Inputs;
using LaundryCleaning.Service.GraphQL.Roles.Services.Interfaces;
using LaundryCleaning.Service.Security;
using LaundryCleaning.Service.Security.Permissions;

namespace LaundryCleaning.Service.GraphQL.Roles.Mutations
{
    [ExtendObjectType(ExtendObjectTypeConstants.Mutation)]
    public class RoleMutations
    {
        [RequirePermission(PermissionConstants.RoleManage)]
        [GraphQLName("addRolePermission")]
        [GraphQLDescription("Add Role Permission.")]
        public async Task<GlobalSuccessResponseCustomModel> AddRolePermission(
            AddRolePermissionInput input,
            [Service] IRoleService service,
            CancellationToken cancellationToken)
        {
            return await service.AddRolePermission(input, cancellationToken);

        }
    }
}
