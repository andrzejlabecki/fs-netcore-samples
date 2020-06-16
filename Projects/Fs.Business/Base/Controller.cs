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

namespace Fs.Business.Base
{
    public class CustomController : ControllerBase
    {
        protected readonly ILogger<CustomController> _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        public CustomController(ILogger<CustomController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        protected void ReportUser(string Method)
        {
            string userID = "<empty>";

            if (_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier) != null)
                userID = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            Fs.Core.Trace.Write("ReportUser() in " + Method, "UserID: " + userID, TraceLevel.Info);
        }

        public async Task<IActionResult> GetAPIResult<T>(string requestURI)
        {
            ReportUser("GetAPIResult - Get()");

            string accessToken = await _httpContextAccessor.HttpContext.GetTokenAsync("access_token");

            Fs.Core.Trace.Write("GetAPIResult()", "Access Token:\r\n " + accessToken, TraceLevel.Info);

            IEnumerable<T> results = null;

            // call api
            var apiClient = new HttpClient();

            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync(requestURI);
            if (!response.IsSuccessStatusCode)
                Fs.Core.Trace.Write("GetWeatherForecast()", "API Call Error: " + response.StatusCode.ToString(), TraceLevel.Info);
            else
                results = response.Content.ReadAsAsync<IEnumerable<T>>().Result;

            return Ok(results);
        }

    }
}
