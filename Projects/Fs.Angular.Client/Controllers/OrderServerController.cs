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
using Fs.Business.Base;

namespace Fs.Client.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class OrderServerController : CustomController
    {
        public OrderServerController(ILogger<OrderServerController> logger, IHttpContextAccessor httpContextAccessor) 
            : base(logger, httpContextAccessor)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await GetAPIResult<Order>("https://fsapi.netpoc.com/order/orders");
        }
    }
}
