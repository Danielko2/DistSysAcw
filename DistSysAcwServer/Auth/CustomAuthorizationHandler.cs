using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace DistSysAcwServer.Auth
{
    /// <summary>
    /// Authorises clients by role
    /// </summary>
    public class CustomAuthorizationHandler : AuthorizationHandler<RolesAuthorizationRequirement>, IAuthorizationHandler
    {
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        public CustomAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAuthorizationRequirement requirement)
        {
            #region Task6
            if (context.User.Identity.IsAuthenticated)
            {
                var hasRequiredRole = context.User.Claims.Any(c => c.Type == ClaimTypes.Role && requirement.AllowedRoles.Contains(c.Value));

                if (hasRequiredRole)
                {
                    context.Succeed(requirement);
                }
                else if (requirement.AllowedRoles.Contains("Admin"))
                {
                    // If the user does not have the Admin role, and the Admin role is required, fail the requirement.
                   
                    context.Fail(new AuthorizationFailureReason(this, "Forbidden. Admin access only."));

                    // Access the current HttpContext from the HttpContextAccessor
                    var httpContext = HttpContextAccessor.HttpContext;
                    // Set the response status code to Forbidden (403)
                    httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                    // Write the custom message to the response
                     httpContext.Response.WriteAsync("Forbidden. Admin access only.");
                }
                else
                {
                    context.Fail();
                }
            }
            else
            {
                context.Fail();
            }
            #endregion

            return Task.CompletedTask;
        }

    }
}