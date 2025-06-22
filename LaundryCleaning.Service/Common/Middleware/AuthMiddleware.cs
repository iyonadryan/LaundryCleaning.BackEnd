using HotChocolate.Resolvers;
using LaundryCleaning.Service.Auth.CustomModels;
using System.Security.Claims;

namespace LaundryCleaning.Service.Common.Middleware
{
    public class AuthMiddleware
    {
        private readonly FieldDelegate _next;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuthMiddleware> _logger;

        public AuthMiddleware(FieldDelegate next, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthMiddleware> logger)
        {
            _next = next;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            //var operationName = context.Operation.Name;

            //_logger.LogInformation($"operationName = {operationName}");

            // Bypass query tertentu seperti authLogin
            // if (string.Equals(operationName, "authLogin", StringComparison.OrdinalIgnoreCase))
            // {
            //     await _next(context);
            //     return;
            // }

            // var user = _httpContextAccessor.HttpContext?.User;
            var httpContext = context.Service<IHttpContextAccessor>()?.HttpContext;
            var user = httpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
            {
                context.ReportError(ErrorBuilder.New()
                    .SetMessage("Unauthorized")
                    .SetCode("AUTH_NOT_AUTHENTICATED")
                    .Build());
                return;
            }
            else
            {
                var username = user.Identity.Name;
                var role = user.FindFirst(ClaimTypes.Role)?.Value;
                var permissions = user.FindAll("permission").Select(p => p.Value).ToList();

                context.ContextData["CurrentUser"] = new CurrentUserCustomModel
                {
                    Username = username,
                    Role = role,
                    Permissions = permissions

                };
            }

                await _next(context);
        }
    }
}
