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
using Fs.Data.Models;

namespace Fs.Client.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OrderServerController : ControllerBase
    {
        private readonly ILogger<OrderServerController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OrderServerController(ILogger<OrderServerController> logger, IHttpContextAccessor httpContextAccessor)
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

            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            Fs.Core.Trace.Write("GetWeatherForecast2()", "Access Token:\r\n " + accessToken, TraceLevel.Info);

            IEnumerable<Order> orders = null;

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
                //var content = await response.Content.ReadAsStringAsync();

                orders = response.Content.ReadAsAsync<IEnumerable<Order>>().Result;

                //Fs.Core.Trace.Write("GetWeatherForecast()", "API Call Result: " + JArray.Parse(content), TraceLevel.Info);
            }

            return Ok(orders);
        }
    }
}
