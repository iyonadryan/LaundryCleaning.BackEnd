using LaundryCleaning.Service.Common.Constants;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.GraphQL.Users.CustomModels;
using LaundryCleaning.Service.GraphQL.Users.Inputs;
using LaundryCleaning.Service.GraphQL.Users.Services.Interfaces;
using LaundryCleaning.Service.Security;
using LaundryCleaning.Service.Security.Permissions;
using Microsoft.EntityFrameworkCore;

namespace LaundryCleaning.Service.GraphQL.Users.Queries
{
    [ExtendObjectType(ExtendObjectTypeConstants.Query)]
    public class UserQueries
    {
        [RequirePermission(PermissionConstants.UserManage)]
        [GraphQLName("getUsers")]
        [GraphQLDescription("Get all user.")]
        public async Task<List<User>> GetUsers(
            [Service] IUserService service,
            CancellationToken cancellationToken)
        {
            return await service.GetUsers(cancellationToken);        
        }
    }
}
