using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Fs.Data.Models;


namespace Fs.Blazor.Service.Pages
{
    public class _HostAuthModel : PageModel
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationStateProvider StateProvider;

        public _HostAuthModel(IHttpContextAccessor httpContextAccessor,
                              ApplicationStateProvider stateProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            StateProvider = stateProvider;
        }

        private void ReportUser(string Method)
        {
            string userID = "<empty>";

            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            System.Diagnostics.Debug.WriteLine("ReportUser() in " + Method, "UserID: " + userID);
        }

        public IActionResult OnGetLogin()
        {
            System.Diagnostics.Debug.WriteLine("\n_Host OnGetLogin");

            var redirectUri = _httpContextAccessor.HttpContext.Request.Query["returnUrl"];
            if (redirectUri.Count == 0)
                redirectUri = _httpContextAccessor.HttpContext.Request.Headers["Referer"];

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
            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

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
                RedirectUri = Url.Content("https://fs-blazor-service.netpoc.com:5001")
            };

            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync(Fs.Data.Models.AppContext.Instance.AuthScheme, authProps);
        }
    }
}
