using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Fs.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Fs.Core.Interfaces.Services;
using Fs.Data.Models;

namespace Fs.Angular.Is4.Client.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LogoutModel> _logger;

        public ISharedConfiguration SharedConfiguration { get; }

        public LogoutModel(SignInManager<ApplicationUser> signInManager, ILogger<LogoutModel> logger, ISharedConfiguration sharedConfiguration)
        {
            _signInManager = signInManager;
            _logger = logger;
            SharedConfiguration = sharedConfiguration;
        }

        public async Task<IActionResult> OnGet(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");

            if (returnUrl == null)
                returnUrl = HttpContext.Request.Headers["Referer"];

            if (Fs.Data.Models.AppContext.Instance.IsExternalLogin)
            {
                string authority = SharedConfiguration.GetOidcLink();
                string logoutUrl = authority + "Identity/Account/Logout?returnUrl=" + Uri.EscapeDataString(returnUrl);

                HttpContext.Response.RedirectToAbsoluteUrl(logoutUrl);
                return null;
            }

            if (returnUrl != null)
            {
                HttpContext.Response.RedirectToAbsoluteUrl(returnUrl);
                return null;
            }
            else
                return RedirectToPage("Login");

        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return RedirectToPage();
            }
        }
    }
}
