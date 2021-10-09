using Microsoft.AspNetCore.Authentication;

namespace AuthTestApi.AuthSchemes
{
    public class AuthSchemeOneOptions : AuthenticationSchemeOptions
    {
        public string ApiKey { get; set; }
    }
}