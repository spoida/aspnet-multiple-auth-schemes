using System;
using Microsoft.Extensions.Options;

namespace AuthTestApi.AuthSchemes
{
    public class AuthSchemeOnePostConfigureOptions : IPostConfigureOptions<AuthSchemeOneOptions>
    {
        public void PostConfigure(string name, AuthSchemeOneOptions options)
        {
            if (string.IsNullOrEmpty(options.ApiKey))
            {
                throw new InvalidOperationException("ApiKey must be provided in options");
            }
        }
    }
}