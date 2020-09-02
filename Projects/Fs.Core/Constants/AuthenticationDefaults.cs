using System;
using System.Collections.Generic;
using System.Text;

namespace Fs.Core.Constants
{
    public static class OpenIdDefaults
    {
        public const string Scheme = "Cookies";
        public const string ChallengeScheme = "oidc";
        public const string ProviderScheme = "Identity.Application";
    }

    public static class AzureDefaults
    {
        public const string AzureScheme = "AzureADCookie";
    }

    public static class AuthenticationDefaults
    {
        public const string ProviderKey = "http://schemas.microsoft.com/identity/claims/identityprovider";
        public const string OidcProvider = "local";
        public const string AzureProvider = "live.com";
    }
}
