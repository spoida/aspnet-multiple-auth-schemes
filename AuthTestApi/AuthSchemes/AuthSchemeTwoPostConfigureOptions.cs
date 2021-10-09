using System;
using Microsoft.Extensions.Options;

namespace AuthTestApi.AuthSchemes
{
    public class AuthSchemeTwoPostConfigureOptions : IPostConfigureOptions<AuthSchemeTwoOptions>
    {
        public void PostConfigure(string name, AuthSchemeTwoOptions options)
        {
            if (string.IsNullOrEmpty(options.Secret))
            {
                throw new InvalidOperationException("Secret must be provided in options");
            }
        }
    }
}