using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Fs.Core.Interfaces.Services;

namespace Fs.Blazor.Is4.Wasm.Client.Server.Controllers
{
    public class OidcConfigurationController : Controller
    {
        private readonly ILogger<OidcConfigurationController> _logger;

        public OidcConfigurationController(ISharedConfiguration sharedConfiguration, ILogger<OidcConfigurationController> logger)
        {
            SharedConfiguration = sharedConfiguration;
            _logger = logger;
        }

        public ISharedConfiguration SharedConfiguration { get; }

        [HttpGet("_configuration/{clientId}")]
        public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        {
            //var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
            var parameters = SharedConfiguration.GetClientParameters(clientId);
            return Ok(parameters);
        }
    }
}
