using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Fs.Core.Constants;
using Fs.Data.Models;

namespace Fs.Business.Authentication
{
    public class IdentityProvider
    {
        static public string GetAuthenticationScheme(string identityProvider)
        {
            switch (identityProvider)
            {
                case AuthenticationDefaults.OidcProvider:
                    return OpenIdDefaults.ChallengeScheme;
                case AuthenticationDefaults.AzureProvider:
                    return AzureADDefaults.AuthenticationScheme;
                default:
                    return null;
            }
        }

        static public int GetProviderIndexByScheme(string authScheme)
        {
            int index = -1;
            Fs.Data.Models.AppContext appContext = Fs.Data.Models.AppContext.Instance;

            if (appContext != null && appContext.AuthSchemes != null && appContext.AuthSchemes.Length > 0)
            {
                foreach(SchemeContext context in appContext.AuthSchemes)
                {
                    index++;

                    if (context.AuthScheme == authScheme)
                        return index;
                }
            }

            return -1;
        }

        static public int GetProviderIndexByLoginPath(string loginPath)
        {
            int index = -1;
            Fs.Data.Models.AppContext appContext = Fs.Data.Models.AppContext.Instance;

            if (appContext != null && appContext.AuthSchemes != null && appContext.AuthSchemes.Length > 0)
            {
                foreach (SchemeContext context in appContext.AuthSchemes)
                {
                    index++;

                    if (context.SignInUri == loginPath)
                        return index;
                }
            }

            return -1;
        }

        static public int GetProviderIndexByLogoutPath(string logoutPath)
        {
            int index = -1;
            Fs.Data.Models.AppContext appContext = Fs.Data.Models.AppContext.Instance;

            if (appContext != null && appContext.AuthSchemes != null && appContext.AuthSchemes.Length > 0)
            {
                foreach (SchemeContext context in appContext.AuthSchemes)
                {
                    index++;

                    if (context.SignOutUri == logoutPath)
                        return index;
                }
            }

            return -1;
        }

        static public int GetProviderIndexByAuthority(string authority)
        {
            int index = -1;
            Fs.Data.Models.AppContext appContext = Fs.Data.Models.AppContext.Instance;

            if (appContext != null && appContext.AuthSchemes != null && appContext.AuthSchemes.Length > 0)
            {
                foreach (SchemeContext context in appContext.AuthSchemes)
                {
                    index++;

                    if (context.Authority == authority)
                        return index;
                }
            }

            return -1;
        }
    }
}
