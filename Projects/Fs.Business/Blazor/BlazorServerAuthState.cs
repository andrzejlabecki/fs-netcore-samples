using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;
using Fs.Core.Constants;

namespace Fs.Blazor
{
    public class InitialApplicationState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public string IdentityCookie { get; set; }
        public int ProviderIndex { get; set; }
    }

    public class ApplicationStateProvider
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public string IdentityCookie { get; set; }
        public int ProviderIndex { get; set; }
    }

    public class BlazorServerAuthState 
        : RevalidatingServerAuthenticationStateProvider
    {
        private readonly ApplicationStateProvider _appStateProvider;
        private IDataProtectionProvider _provider;

        public BlazorServerAuthState(
            ILoggerFactory loggerFactory,
            ApplicationStateProvider stateProvider,
            IDataProtectionProvider provider)
            : base(loggerFactory)
        {
            //Cache = cache;
            _appStateProvider = stateProvider;
            _provider = provider;
        }

        protected AuthenticationTicket DecryptIdentityCookie()
        {
            if (_appStateProvider == null ||
                _appStateProvider.IdentityCookie == null ||
                _appStateProvider.IdentityCookie.Length == 0)
                return null;

            //Get the encrypted cookie value
            string cookieValue = _appStateProvider.IdentityCookie;

            //Get a data protector to use with either approach
            var dataProtector = _provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", AzureDefaults.AzureScheme, "v2");

            //Get the decrypted cookie as a Authentication Ticket
            TicketDataFormat ticketDataFormat = new TicketDataFormat(dataProtector);
            AuthenticationTicket ticket = ticketDataFormat.Unprotect(cookieValue);

            return ticket;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            //return base.GetAuthenticationStateAsync();
            if (Fs.Data.Models.AppContext.Instance.AuthSchemes[_appStateProvider.ProviderIndex].AuthScheme == OpenIdDefaults.ChallengeScheme)
                return base.GetAuthenticationStateAsync();

            AuthenticationTicket ticket = DecryptIdentityCookie();

            if (ticket == null)
                return base.GetAuthenticationStateAsync();

            ClaimsPrincipal user = new ClaimsPrincipal(ticket.Principal.Identity);

            return Task.FromResult(new AuthenticationState(user));
        }

        protected override TimeSpan RevalidationInterval
            => TimeSpan.FromSeconds(60); // TODO read from config

        protected override Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            var sid =
                authenticationState.User.Claims
                .Where(c => c.Type.Equals("sid"))
                .Select(c => c.Value)
                .FirstOrDefault();

            var name =
                authenticationState.User.Claims
                .Where(c => c.Type.Equals("name"))
                .Select(c => c.Value)
                .FirstOrDefault() ?? string.Empty;
            System.Diagnostics.Debug.WriteLine($"\nValidate: {name} / {sid}");

            if (sid != null)
            {
                System.Diagnostics.Debug.WriteLine($"NowUtc: {DateTimeOffset.UtcNow.ToString("o")}");
                System.Diagnostics.Debug.WriteLine($"ExpUtc: {_appStateProvider.Expiration.ToString("o")}");

                if(DateTimeOffset.UtcNow >= _appStateProvider.Expiration)
                {
                    System.Diagnostics.Debug.WriteLine($"*** EXPIRED ***");
                    return Task.FromResult(false);
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"(not in cache)");
            }

            return Task.FromResult(true);
        }
    }
}
