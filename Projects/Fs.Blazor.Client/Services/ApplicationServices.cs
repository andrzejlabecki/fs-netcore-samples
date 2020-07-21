using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.Extensions.Logging;
using Fs.Business.Controllers;
using Fs.Models;
using Fs.Core.Interfaces.Services;
using System.Linq;

namespace Fs.Client.Services
{
    public class ForecastService : BaseWeatherForecastController
    {
        public ForecastService(ILogger<ForecastService> logger, IHttpContextAccessor httpContextAccessor)
            : base(logger, httpContextAccessor)
        {
        }

        public new WeatherForecast [] Get()
        {
            WeatherForecast[] forecasts = base.Get().ToArray();

            return forecasts;
        }
    }

    public class OrderService
    {
        public string AccessToken { get; set; }

        public async Task<Order[]> GetOrdersAsync()
        {
            string accessToken = AccessToken;

            IEnumerable<Order> results = null;

            // call api
            var apiClient = new HttpClient();

            apiClient.SetBearerToken(accessToken);

            var response = await apiClient.GetAsync("https://fs-api.netpoc.com/order/orders");

            if (!response.IsSuccessStatusCode)
            {
                Fs.Core.Trace.Write("GetOrdersAsync()", "API Call Error: " + response.StatusCode.ToString(), TraceLevel.Info);
                return null;
            }
            else
            {
                results = response.Content.ReadAsAsync<IEnumerable<Order>>().Result;
                return results.Cast<Order>().ToArray();
            }
        }
    }
}
