using System;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Fs.Core.Constants;
using Fs.Business.Authentication;

namespace Fs.Business.Pages
{
    public class HostAuthModel : PageModel
    {
        private int _providerIndex = 0;
        private string _identityCookie = null;
        private string _accessToken = null;
        private string _refreshToken = null;
        DateTimeOffset _expiration = new DateTimeOffset();

        public int ProviderIndex
        {
            get { return _providerIndex; }
        }

        public string IdentityCookie
        {
            get { return _identityCookie; }
        }

        public string AccessToken
        {
            get { return _accessToken; }
        }

        public string RefreshToken
        {
            get { return _refreshToken; }
        }

        public DateTimeOffset Expiration
        {
            get { return _expiration; }
        }

        private void ReportUser(string Method)
        {
            string userID = "<empty>";

            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            System.Diagnostics.Debug.WriteLine("ReportUser() in " + Method, "UserID: " + userID);
        }

        public async Task<IActionResult> OnGet()
        {
            //System.Diagnostics.Debug.WriteLine("\n_Host Get");

            Fs.Data.Models.AppContext appContext = Fs.Data.Models.AppContext.Instance;
            string authScheme = null;

            string referer = HttpContext.Request.Headers["Referer"];

            if (referer != null && referer.Length > 0)
            {
                _providerIndex = IdentityProvider.GetProviderIndexByAuthority(referer);
                if (_providerIndex == -1)
                {
                    _providerIndex = 0;
                    return Page();
                }

                authScheme = appContext.AuthSchemes[_providerIndex].AuthScheme;
            }
            else
            {
                bool isAuthenticated = HttpContext.User.Identity.IsAuthenticated;
                if (isAuthenticated)
                {
                    string identityProvider = HttpContext.User.Claims
                                                    .Where(c => c.Type.Equals(AuthenticationDefaults.ProviderKey))
                                                    .Select(c => c.Value)
                                                    .FirstOrDefault() ?? string.Empty;

                    authScheme = IdentityProvider.GetAuthenticationScheme(identityProvider);
                
                    _providerIndex = IdentityProvider.GetProviderIndexByScheme(authScheme);
                }
            }

            if (authScheme == null)
                return Page();

            if (_providerIndex == -1)
                // something went wrong
                throw new Exception("HostAuthModel: authentication scheme '" + authScheme + "' isn't initialized in app context.");

            var authResult = await HttpContext.AuthenticateAsync(authScheme);

            if (authResult.Succeeded)
            {
                _expiration = authResult.Properties.ExpiresUtc.Value;

                if (authScheme == AzureADDefaults.AuthenticationScheme)
                {
                    var refererUri = HttpContext.Request.Headers["Referer"];

                    if (refererUri == appContext.AuthSchemes[_providerIndex].Authority)
                    {
                        if (HttpContext.Request.Query.Count == 0)
                        {
                            var @params = new NameValueCollection
                            {
                                // Azure AD will return an authorization code. 
                                {"response_type", appContext.AuthSchemes[_providerIndex].ResponseType},
                                // You get the client id when you register your Azure client app.
                                {"client_id", appContext.AuthSchemes[_providerIndex].ClientId},
                                // You get the resource URI (client id) when you register your Azure API app.
                                {"resource", appContext.AuthSchemes[_providerIndex].ResourceId},
                                //After user authenticates, Azure AD will redirect back to the web app
                                {"redirect_uri", appContext.RedirectUri}
                            };

                            //Create sign-in query string
                            var queryString = HttpUtility.ParseQueryString(string.Empty);
                            queryString.Add(@params);

                            // Redirect to authority
                            var authorityUri = String.Format("{0}{1}{2}?{3}", 
                                appContext.AuthSchemes[_providerIndex].Authority, 
                                appContext.AuthSchemes[_providerIndex].TenantId, 
                                appContext.AuthSchemes[_providerIndex].AuthorizePath, queryString);
                            Response.Redirect(authorityUri);
                        }
                        else
                        {
                            string authorityUri = String.Format("{0}{1}", 
                                                                appContext.AuthSchemes[_providerIndex].Authority, 
                                                                appContext.AuthSchemes[_providerIndex].TenantId);

                            // Get the auth code
                            string code = Request.Query[appContext.AuthSchemes[_providerIndex].CodeField];

                            // Get auth token from auth code
                            TokenCache tokenCache = new TokenCache();

                            AuthenticationContext authContext = new AuthenticationContext(authorityUri, tokenCache);

                            ClientCredential clientCredential = new ClientCredential(appContext.AuthSchemes[_providerIndex].ClientId, 
                                                                                     appContext.AuthSchemes[_providerIndex].ClientSecret);

                            AuthenticationResult aResult = await authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(appContext.RedirectUri), clientCredential);

                            _accessToken = aResult.AccessToken;
                        }
                    }
                }
                else if (authScheme == OpenIdDefaults.ChallengeScheme)
                {
                    var refererUri = HttpContext.Request.Headers["Referer"];
                    _accessToken = await HttpContext.GetTokenAsync("access_token");
                    _refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                }
            }

            return Page();
        }

        public IActionResult OnGetLogin()
        {
            System.Diagnostics.Debug.WriteLine("\n_Host OnGetLogin");

            var redirectUri = HttpContext.Request.Query["returnUrl"];
            if (redirectUri.Count == 0)
                redirectUri = HttpContext.Request.Headers["Referer"];

            if (redirectUri.Count == 0)
                redirectUri = "~/";

            var authProps = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1),
                //ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(30),
                RedirectUri = Url.Content(redirectUri)
            };

            string loginPath = HttpContext.Request.Path;
            _providerIndex = IdentityProvider.GetProviderIndexByLoginPath(loginPath);

            if (_providerIndex == -1)
                // something went wrong
                throw new Exception("HostAuthModel: login path '" + loginPath + "' isn't initialized in app context.");

            return Challenge(authProps, Fs.Data.Models.AppContext.Instance.AuthSchemes[_providerIndex].AuthScheme);
        }

        public async Task OnGetLogout()
        {
            System.Diagnostics.Debug.WriteLine("\n_Host OnGetLogout");

            string userID = null;
            if (HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var sid = "";

            if (User.Identity.IsAuthenticated)
            {
                sid = User.Claims
                    .Where(c => c.Type.Equals("sid"))
                    .Select(c => c.Value)
                    .FirstOrDefault();

                System.Diagnostics.Debug.WriteLine($"sid: {sid}");
            }

            var authProps = new AuthenticationProperties
            {
                RedirectUri = Url.Content(Fs.Data.Models.AppContext.Instance.RedirectUri)
            };

            string logoutPath = HttpContext.Request.Path;
            _providerIndex = IdentityProvider.GetProviderIndexByLogoutPath(logoutPath);

            if (_providerIndex == -1)
                // something went wrong
                throw new Exception("HostAuthModel: logout path '" + logoutPath + "' isn't initialized in app context.");

            await HttpContext.SignOutAsync(OpenIdDefaults.Scheme);
            await HttpContext.SignOutAsync(Fs.Data.Models.AppContext.Instance.AuthSchemes[_providerIndex].AuthScheme, authProps);
        }
    }
}
