using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Logging;
using Fs.Business.Controllers;
using Fs.Data.Models;
using Fs.Core.Interfaces.Services;

namespace Fs.Client.Controllers
{
    public class WeatherForecastController : BaseWeatherForecastController
    {
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpContextAccessor httpContextAccessor)
            : base(logger, httpContextAccessor)
        {
        }
    }

    public class OidcContextController : BaseOidcContextController
    {
        public OidcContextController(SignInManager<ApplicationUser> signInManager, ILogger<OidcContextController> logger, IHttpContextAccessor httpContextAccessor, ISharedConfiguration sharedConfiguration)
            : base(signInManager, logger, httpContextAccessor, sharedConfiguration)
        {
        }
    }

    public class OrderServerController : BaseOrderServerController
    {
        public OrderServerController(ILogger<OrderServerController> logger, IHttpContextAccessor httpContextAccessor)
            : base(logger, httpContextAccessor)
        {
            _baseURL = "https://fs-api.netpoc.com/order";
        }
    }
}
