﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Fs.Blazor.Client.Models;


namespace Fs.Blazor.Client.Pages
{
    public class _HostAuthModel : PageModel
    {
        public readonly BlazorServerAuthStateCache Cache;
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private readonly SignInManager<IdentityUser> SignInManager;

        public _HostAuthModel(BlazorServerAuthStateCache cache, IHttpContextAccessor httpContextAccessor/*, SignInManager<IdentityUser> signInManager*/)
        {
            Cache = cache;
            _httpContextAccessor = httpContextAccessor;
            //SignInManager = signInManager;
        }

        private void ReportUser(string Method)
        {
            string userID = "<empty>";

            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            System.Diagnostics.Debug.WriteLine("ReportUser() in " + Method, "UserID: " + userID);
        }


        public async Task<IActionResult> OnGet()
        {
            System.Diagnostics.Debug.WriteLine($"\n_Host OnGet IsAuth? {User.Identity.IsAuthenticated}");
            ReportUser("OnGet()");

            if (User.Identity.IsAuthenticated)
            {
                var sid = User.Claims
                    .Where(c => c.Type.Equals("sid"))
                    .Select(c => c.Value)
                    .FirstOrDefault();

                System.Diagnostics.Debug.WriteLine($"sid: {sid}");

                if (sid != null && !Cache.HasSubjectId(sid))
                {
                    string authScheme = "oidc";
                    var authResult = await HttpContext.AuthenticateAsync("oidc");
                    if (authResult.Succeeded == false)
                    {
                        authResult = await HttpContext.AuthenticateAsync("oidc1");
                        authScheme = "oidc1";
                    }

                    DateTimeOffset expiration = authResult.Properties.ExpiresUtc.Value;
                    string accessToken = await HttpContext.GetTokenAsync("access_token");
                    string refreshToken = await HttpContext.GetTokenAsync("refresh_token");
                    Cache.Add(sid, expiration, accessToken, refreshToken, authScheme);
                }
            }
            return Page();
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

            return Challenge(authProps, "oidc");
        }

        public IActionResult OnGetLogin1()
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

            return Challenge(authProps, "oidc1");
        }

        public async Task OnGetLogout()
        {
            System.Diagnostics.Debug.WriteLine("\n_Host OnGetLogout");

            string userID = null;
            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var sid = "";
            BlazorServerAuthData serverAuthData = null;

            if (User.Identity.IsAuthenticated)
            {
                sid = User.Claims
                    .Where(c => c.Type.Equals("sid"))
                    .Select(c => c.Value)
                    .FirstOrDefault();

                serverAuthData = Cache.Get(sid);
                System.Diagnostics.Debug.WriteLine($"sid: {sid}");
            }

            var authProps = new AuthenticationProperties
            {
                RedirectUri = Url.Content("~/")
            };

            await HttpContext.SignOutAsync("Cookies");

            if (serverAuthData != null && serverAuthData.AuthScheme == "oidc1")
                HttpContext.Response.RedirectToAbsoluteUrl(string.Format("https://fs-angular-is4.netpoc.com/OidcLogout?&userID={0}", userID));
            else
                await HttpContext.SignOutAsync("oidc", authProps);
        }
    }
}