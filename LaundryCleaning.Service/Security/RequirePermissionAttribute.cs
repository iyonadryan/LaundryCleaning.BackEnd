using HotChocolate.Types.Descriptors;
using LaundryCleaning.Service.Auth.CustomModels;
using System.Reflection;
using System.Security.Claims;

namespace LaundryCleaning.Service.Security
{
    public class RequirePermissionAttribute : ObjectFieldDescriptorAttribute
    {
        private readonly string _requiredPermission;

        public RequirePermissionAttribute(string requiredPermission)
        {
            _requiredPermission = requiredPermission;
        }

        protected override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            descriptor.Use(next => async ctx =>
            {
                if (!ctx.ContextData.TryGetValue("CurrentUser", out var userObj) || userObj is not CurrentUserCustomModel user)
                {
                    throw new GraphQLException("Unauthorized");
                }

                if (user.Permissions == null || !user.Permissions.Contains(_requiredPermission))
                {
                    throw new GraphQLException("Forbidden: missing required permission.");
                }

                await next(ctx);
            });
        }
    }

}
