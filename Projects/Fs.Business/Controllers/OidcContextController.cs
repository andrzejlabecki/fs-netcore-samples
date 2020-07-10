using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fs.Core.Interfaces.Services;
using Fs.Data.Models;
using Fs.Models;
using Microsoft.AspNetCore.Authentication;

namespace Fs.Business.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseOidcContextController : CustomController
    {
        private readonly SignInManager<ApplicationUser> _signInManager;

        public BaseOidcContextController(SignInManager<ApplicationUser> signInManager, ILogger<BaseOidcContextController> logger, IHttpContextAccessor httpContextAccessor, ISharedConfiguration sharedConfiguration)
            : base(logger, httpContextAccessor)
        {
            _signInManager = signInManager;
            SharedConfiguration = sharedConfiguration;
        }

        public ISharedConfiguration SharedConfiguration { get; }

        [HttpGet]
        public OidcContext Get()
        {
            ReportUser("OidcContext - Get()");

            OidcContext context = new OidcContext
            {
                IsExternalLogin = Fs.Data.Models.AppContext.Instance.IsExternalLogin,
                Authority = SharedConfiguration.GetOidcLink()
            };

            return context;
        }

        [HttpGet("signout")]
        public async Task<IActionResult> Signout()
        {
            // experimental method
            ReportUser("OidcContext - Signout()");

            await _signInManager.SignOutAsync();

            await HttpContext.SignOutAsync("oidc");
            await HttpContext.SignOutAsync("Identity.Application");

            return null;
        }
    }
}
