using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Fs.Models;

namespace Fs.Blazor.Client.Data
{
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
