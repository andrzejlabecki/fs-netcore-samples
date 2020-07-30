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
using Fs.Core.Interfaces.Services;
using Fs.Core.Constants;

namespace Fs.Business.Pages
{
    public class HostAuthModel : PageModel
    {
        private string _accessToken = null;
        private string _refreshToken = null;
        DateTimeOffset _expiration = new DateTimeOffset();

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
            System.Diagnostics.Debug.WriteLine("\n_Host OnGetLogin");

            Fs.Data.Models.AppContext appContext = Fs.Data.Models.AppContext.Instance;

            var authResult = await HttpContext.AuthenticateAsync(appContext.AuthScheme);

            if (authResult.Succeeded)
            {
                _expiration = authResult.Properties.ExpiresUtc.Value;

                if (appContext.AuthScheme == AzureADDefaults.AuthenticationScheme)
                {
                    var refererUri = HttpContext.Request.Headers["Referer"];

                    if (refererUri == appContext.Authority)
                    {
                        if (HttpContext.Request.Query.Count == 0)
                        {
                            var @params = new NameValueCollection
                            {
                                // Azure AD will return an authorization code. 
                                {"response_type", appContext.ResponseType},
                                // You get the client id when you register your Azure client app.
                                {"client_id", appContext.ClientId},
                                // You get the resource URI (client id) when you register your Azure API app.
                                {"resource", appContext.ResourceId},
                                //After user authenticates, Azure AD will redirect back to the web app
                                {"redirect_uri", appContext.RedirectUri}
                            };

                            //Create sign-in query string
                            var queryString = HttpUtility.ParseQueryString(string.Empty);
                            queryString.Add(@params);

                            // Redirect to authority
                            var authorityUri = String.Format("{0}{1}{2}?{3}", 
                                appContext.Authority, appContext.TenantId, appContext.AuthorizePath, queryString);
                            Response.Redirect(authorityUri);
                        }
                        else
                        {
                            string authorityUri = String.Format("{0}{1}", appContext.Authority, appContext.TenantId);

                            // Get the auth code
                            string code = Request.Query[appContext.CodeField];

                            // Get auth token from auth code
                            TokenCache tokenCache = new TokenCache();

                            AuthenticationContext authContext = new AuthenticationContext(authorityUri, tokenCache);

                            ClientCredential clientCredential = new ClientCredential(appContext.ClientId, appContext.ClientSecret);

                            AuthenticationResult aResult = await authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri(appContext.RedirectUri), clientCredential);

                            _accessToken = aResult.AccessToken;
                        }
                    }
                }
                else if (appContext.AuthScheme == OpenIdDefaults.ChallengeScheme)
                {
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

            return Challenge(authProps, Fs.Data.Models.AppContext.Instance.AuthScheme);
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

            await HttpContext.SignOutAsync(OpenIdDefaults.Scheme);
            await HttpContext.SignOutAsync(Fs.Data.Models.AppContext.Instance.AuthScheme, authProps);
        }
    }
}
