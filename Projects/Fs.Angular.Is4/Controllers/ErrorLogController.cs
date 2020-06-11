using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fs.Core.Contracts;
using Fs.Core.Interfaces.Services;
using Fs.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fs.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ErrorLogController : ControllerBase
    {

        private readonly ILogger<ErrorLogController> _logger;
        private readonly LoggerContext _context;
        private readonly IErrorLogService _service;

        public ErrorLogController(ILogger<ErrorLogController> logger, LoggerContext context, IErrorLogService service)
        {
            _logger = logger;
            _context = context;
            _service = service;
        }

        // POST api/<controller>
 /*       [HttpPost]
        public void Post([FromBody]ErrorLog value)
        {
            _logger.LogError($"Type: {value.Type} \r\nMessage: {value.Message} \r\nCustom Message: {value.CustomMessage} \r\nPage: {value.Page} \r\nStack:{value.Stack}");
        }
*/

        [HttpPost]
        public async Task<IActionResult> Post([FromBody]ErrorLogDto value)
        {
            _logger.LogError($"Type: {value.Type} \r\nMessage: {value.Message} \r\nCustom Message: {value.CustomMessage} \r\nPage: {value.Page} \r\nStack:{value.Stack}");

            await _service.SaveErrorLog(value);

            return Ok();
        }
    }
}
