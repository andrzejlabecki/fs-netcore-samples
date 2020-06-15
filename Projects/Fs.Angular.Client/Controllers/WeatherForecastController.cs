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

namespace Fs.Client.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        private void ReportUser(string Method)
        {
            string userID = "<empty>";

            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Fs.Core.Trace.Write("ReportUser() in " + Method, "UserID: " + userID, TraceLevel.Info);
        }

        [HttpGet]
        public async Task<IActionResult> Get()
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


            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            Fs.Core.Trace.Write("GetWeatherForecast2()", "Access Token:\r\n " + accessToken, TraceLevel.Info);

            // call api
            var apiClient = new HttpClient();

            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync("https://fsapi.netpoc.com/order/orders");
            if (!response.IsSuccessStatusCode)
            {
                Fs.Core.Trace.Write("GetWeatherForecast()", "API Call Error: " + response.StatusCode.ToString(), TraceLevel.Info);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Fs.Core.Trace.Write("GetWeatherForecast()", "API Call Result: " + JArray.Parse(content), TraceLevel.Info);
            }


            //CustomMessageException ex = new CustomMessageException() { ExceptionMessage = "I couldn't execute your request due to bad search criteria." };
            //throw ex;

            return Ok(forecasts);
        }
    }
}
