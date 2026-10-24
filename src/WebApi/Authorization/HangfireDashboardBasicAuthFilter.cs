using Hangfire.Annotations;
using Hangfire.Dashboard;
using System.Security.Cryptography;
using System.Text;

namespace WebApi.Authorization
{
    public sealed class HangfireDashboardBasicAuthFilter : IDashboardAuthorizationFilter
    {
        private readonly string _username;
        private readonly string _password;

        public HangfireDashboardBasicAuthFilter(
            IConfiguration configuration)
        {
            _username = configuration["HangfireDashboard:Username"]
            ?? throw new InvalidOperationException(
                "Hangfire dashboard username is not configured.");

            _password = configuration["HangfireDashboard:Password"]
                ?? throw new InvalidOperationException(
                    "Hangfire dashboard password is not configured.");
        }


        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            var authorizationHeader =
                httpContext.Request.Headers.Authorization.ToString();

            if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                return Challenge(httpContext);
            }

            try
            {
                var encodedCredentials = authorizationHeader["Basic ".Length..];

                var credentials = Encoding.UTF8.GetString(
                    Convert.FromBase64String(encodedCredentials));

                var separatorIndex = credentials.IndexOf(':');

                if (separatorIndex < 0)
                {
                    return Challenge(httpContext);
                }

                var username = credentials[..separatorIndex];
                var password = credentials[(separatorIndex + 1)..];

                var usernameMatches = CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(username),
                    Encoding.UTF8.GetBytes(_username));

                var passwordMatches = CryptographicOperations.FixedTimeEquals(
                    Encoding.UTF8.GetBytes(password),
                    Encoding.UTF8.GetBytes(_password));

                if (usernameMatches && passwordMatches)
                {
                    return true;
                }
            }
            catch (FormatException)
            {
                // The Authorization header contains invalid Base64.
            }

            return Challenge(httpContext);
        }

        private static bool Challenge(HttpContext context)
        {
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;

            context.Response.Headers["WWW-Authenticate"] =
                "Basic realm=\"Hangfire Dashboard\", charset=\"UTF-8\"";

            return false;
        }
    }
}