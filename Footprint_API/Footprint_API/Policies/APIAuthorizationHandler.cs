using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System.Threading.Tasks;

namespace Footprint_API.Policies
{
    public class APIAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            if (context.User.HasClaim("scope", "scope.fullaccess"))
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;

        }
    }
}
