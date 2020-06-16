using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json.Linq;
using Fs.Business.Base;

namespace Fs.Client.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BaseWeatherForecastController : CustomController
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public BaseWeatherForecastController(ILogger<BaseWeatherForecastController> logger, IHttpContextAccessor httpContextAccessor)
            : base(logger, httpContextAccessor)
        {
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            ReportUser("Forecast - Get()");

            var rng = new Random();

            IEnumerable<WeatherForecast> forecasts = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();

            return forecasts;
        }
    }
}
