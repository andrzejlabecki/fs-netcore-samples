using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Fs.Core.Constants;

namespace Fs.Blazor
{
    public class RevalidatingIdentityAuthenticationStateProvider<TUser>
        : RevalidatingServerAuthenticationStateProvider where TUser : class
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IdentityOptions _options;
        private readonly ApplicationStateProvider _appStateProvider;
        private IDataProtectionProvider _provider;

        public RevalidatingIdentityAuthenticationStateProvider(
            ILoggerFactory loggerFactory,
            IServiceScopeFactory scopeFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ApplicationStateProvider appStateProvider,
            IDataProtectionProvider provider)
            : base(loggerFactory)
        {
            _scopeFactory = scopeFactory;
            _options = optionsAccessor.Value;
            _appStateProvider = appStateProvider;
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
            var dataProtector = _provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", OpenIdDefaults.ProviderScheme, "v2");

            //Get the decrypted cookie as a Authentication Ticket
            TicketDataFormat ticketDataFormat = new TicketDataFormat(dataProtector);
            AuthenticationTicket ticket = ticketDataFormat.Unprotect(cookieValue);

            return ticket;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            AuthenticationTicket ticket = DecryptIdentityCookie();
            //return base.GetAuthenticationStateAsync();

            if (ticket == null)
                return base.GetAuthenticationStateAsync();

            ClaimsPrincipal user = new ClaimsPrincipal(ticket.Principal.Identity);

            return Task.FromResult(new AuthenticationState(user));
        }

        protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

        protected override async Task<bool> ValidateAuthenticationStateAsync(
            AuthenticationState authenticationState, CancellationToken cancellationToken)
        {
            // Get the user manager from a new scope to ensure it fetches fresh data
            var scope = _scopeFactory.CreateScope();
            try
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TUser>>();
                return await ValidateSecurityStampAsync(userManager, authenticationState.User);
            }
            finally
            {
                if (scope is IAsyncDisposable asyncDisposable)
                {
                    await asyncDisposable.DisposeAsync();
                }
                else
                {
                    scope.Dispose();
                }
            }
        }

        private async Task<bool> ValidateSecurityStampAsync(UserManager<TUser> userManager, ClaimsPrincipal principal)
        {
            var user = await userManager.GetUserAsync(principal);
            if (user == null)
            {
                return false;
            }
            else if (!userManager.SupportsUserSecurityStamp)
            {
                return true;
            }
            else
            {
                var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
                var userStamp = await userManager.GetSecurityStampAsync(user);
                return principalStamp == userStamp;
            }
        }
    }
}
