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
    public class AuthSchemeOneHandler : AuthenticationHandler<AuthSchemeOneOptions>
    {
        public const string AuthSchemeOne = nameof(AuthSchemeOne);
        
        public AuthSchemeOneHandler(IOptionsMonitor<AuthSchemeOneOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
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
            
            if(!AuthSchemeOne.Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
            {
                // Not AuthSchemeOne authentication header
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            if (!Options.ApiKey.Equals(headerValue.Parameter))
            {
                return Task.FromResult(AuthenticateResult.Fail("Invalid AuthSchemeOne authentication header"));
            }
            
            var claims = new[] { new Claim(ClaimTypes.Name, AuthSchemeOne) };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}