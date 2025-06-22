using HotChocolate.Subscriptions;
using LaundryCleaning.Service.Common.Constants;
using LaundryCleaning.Service.Common.Models.Entities;
using LaundryCleaning.Service.GraphQL.Users.CustomModels;
using LaundryCleaning.Service.GraphQL.Users.Inputs;
using LaundryCleaning.Service.GraphQL.Users.Services.Interfaces;
using LaundryCleaning.Service.Security;
using LaundryCleaning.Service.Security.Permissions;

namespace LaundryCleaning.Service.GraphQL.Users.Mutations
{
    [ExtendObjectType(ExtendObjectTypeConstants.Mutation)]
    public class UserMutations
    {
        [GraphQLName("createUser")]
        [GraphQLDescription("Create new user.")]
        public async Task<CreateUserCustomModel> CreateUser(
            CreateUserInput input,
            [Service] IUserService service,
            CancellationToken cancellationtoken)
        {
            return await service.CreateUser(input, cancellationtoken);
        }

        [RequirePermission(PermissionConstants.UserManage)]
        [GraphQLName("sendUserNotification")]
        [GraphQLDescription("Send User Notification.")]
        public async Task<string> SendUserNotification(
            string input,
            [Service] IUserService service,
            CancellationToken cancellationtoken)
        {
            return await service.SendUserNotification(input, cancellationtoken);
        }
    }
}
