using Microsoft.AspNetCore.Authentication;

namespace AuthTestApi.AuthSchemes
{
    public class AuthSchemeTwoOptions : AuthenticationSchemeOptions
    {
        public string Secret { get; set; }
    }
}