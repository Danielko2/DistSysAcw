using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace DistSysAcwServer.Auth
{
    /// <summary>
    /// Authenticates clients by API Key
    /// </summary>
    public class CustomAuthenticationHandler
        : AuthenticationHandler<AuthenticationSchemeOptions>, IAuthenticationHandler
    {
        private Models.UserContext DbContext { get; set; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }

        public CustomAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            Models.UserContext dbContext,
            IHttpContextAccessor httpContextAccessor)
            : base(options, logger, encoder, clock) 
        {
            DbContext = dbContext;
            HttpContextAccessor = httpContextAccessor;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            #region Task5
            // TODO: Find if a header ‘ApiKey’ exists, and if it does, check the database to determine if the given API Key is valid
            // Then create the correct Claims, add these to a ClaimsIdentity, create a ClaimsPrincipal from the identity
            // Then use the Principal to generate a new AuthenticationTicket to return a Success AuthenticateResult

            // Read the ApiKey from the request headers
            if (!Request.Headers.TryGetValue("ApiKey", out var apiKeyHeaderValues))
            {
                return AuthenticateResult.NoResult();
            }

            var providedApiKey = apiKeyHeaderValues.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(providedApiKey))
            {
                return AuthenticateResult.Fail("Unauthorized: ApiKey is missing.");
            }

            // Check if the ApiKey is valid
            var user = await DbContext.Users.SingleOrDefaultAsync(u => u.ApiKey == providedApiKey);

            if (user == null)
            {
                return AuthenticateResult.Fail("Unauthorized: Invalid ApiKey.");
            }

            // Create claims based on the authenticated user information
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.UserName),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("ApiKey", user.ApiKey)
    };

            // Generate a ClaimsIdentity with the claims and an authentication type
            var identity = new ClaimsIdentity(claims, Scheme.Name);

            // Generate a ClaimsPrincipal from the identity
            var principal = new ClaimsPrincipal(identity);

            // Generate a new AuthenticationTicket with the principal and the scheme name
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            // Return a Success AuthenticateResult with the ticket
            return AuthenticateResult.Success(ticket);
            #endregion
        }


        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            byte[] messagebytes = Encoding.ASCII.GetBytes("Task 5 Incomplete");
            Context.Response.StatusCode = 401;
            Context.Response.ContentType = "application/json";
            await Context.Response.Body.WriteAsync(messagebytes, 0, messagebytes.Length);
            await HttpContextAccessor.HttpContext.Response.CompleteAsync();
        }
    }
}