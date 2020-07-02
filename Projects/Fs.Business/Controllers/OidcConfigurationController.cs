using System.Diagnostics;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fs.Core.Interfaces.Services;

namespace Fs.Business.Controllers
{
    public class OidcConfigurationController : Controller
    {
        private readonly ILogger<OidcConfigurationController> logger;

        public OidcConfigurationController(ISharedConfiguration sharedConfiguration, ILogger<OidcConfigurationController> _logger)
        {
            SharedConfiguration = sharedConfiguration;
            logger = _logger;
        }

        public ISharedConfiguration SharedConfiguration { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            Fs.Core.Trace.Write("GetClientRequestParameters()", "ClientID: " + clientId, TraceLevel.Info);
            var parameters = SharedConfiguration.GetClientParameters(clientId);
            return Ok(parameters);
        }
    }
}
