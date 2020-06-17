using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fs.Data.Models;

namespace Fs.Business.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class BaseOrderServerController : CustomController
    {
        public BaseOrderServerController(ILogger<BaseOrderServerController> logger, IHttpContextAccessor httpContextAccessor) 
            : base(logger, httpContextAccessor)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return await GetAPIResult<Order>(_baseURL + "/orders");
        }
    }
}
