using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Logging;
using Fs.Business.Controllers;

namespace Fs.Client.Controllers
{
    public class WeatherForecastController : BaseWeatherForecastController
    {
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpContextAccessor httpContextAccessor)
            : base(logger, httpContextAccessor)
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
