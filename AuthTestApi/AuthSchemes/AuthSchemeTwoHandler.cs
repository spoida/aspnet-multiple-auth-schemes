using System;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AuthTestApi.AuthSchemes
{
    public class AuthSchemeTwoHandler : AuthenticationHandler<AuthSchemeTwoOptions>
    {
        public const string AuthSchemeTwo = nameof(AuthSchemeTwo);
        
        public AuthSchemeTwoHandler(IOptionsMonitor<AuthSchemeTwoOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                // Authorization header not in request
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            
            if(!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out AuthenticationHeaderValue headerValue))
            {
                // Invalid Authorization header
                return Task.FromResult(AuthenticateResult.NoResult());
            }
            
            if(!AuthSchemeTwo.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                // Not AuthSchemeTwo authentication header
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!Options.Secret.Equals(headerValue.Parameter))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid AuthSchemeTwo authentication header"));
            }
            
            var claims = new[] { new Claim(ClaimTypes.Name, AuthSchemeTwo) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}