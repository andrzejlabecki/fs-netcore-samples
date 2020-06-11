using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;

namespace Fs.App.Pages
{
    public class OidcLogoutModel : PageModel
    {
        private readonly ILogger<OidcLogoutModel> _logger;

        public OidcLogoutModel(ILogger<OidcLogoutModel> logger)
        {
            _logger = logger;
        }

        public async Task OnGet()
        {
            string returnUrl = null;
            string userID = null;

            var query = HttpContext.Request.Query;

            returnUrl = HttpContext.Request.Headers["Referer"];

            userID = query["userID"];

            Fs.Core.Trace.Write(string.Format("OidcLogout::OnGet(), Return URL: {0}, UserID: {1}", returnUrl, userID), TraceLevel.Info);

            if (returnUrl != null)
            {
                // check URL from white list here, CORS should be set accordingly, maybe pass a secret here
                if (returnUrl == "https://blazor2.netpoc.com/")
                {
                    await HttpContext.SignOutAsync("Identity.Application");

                    HttpContext.Response.RedirectToAbsoluteUrl(returnUrl+ "loggedout");
                }
            }
        }
    }
}