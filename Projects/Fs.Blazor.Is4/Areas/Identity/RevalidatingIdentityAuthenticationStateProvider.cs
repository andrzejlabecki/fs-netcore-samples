using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Fs.Blazor.Is4;

namespace Fs.Blazor.Is4.Areas.Identity
{
    public class ApplicationStateReader
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IDataProtectionProvider _provider;

        public ApplicationStateReader(IHttpContextAccessor httpContextAccessor,
                                      IDataProtectionProvider provider)
        {
            _httpContextAccessor = httpContextAccessor;
            _provider = provider;
        }

        public string DecryptIdentityCookie()
        {
            if (_httpContextAccessor.HttpContext == null)
                return null;

            //Get the encrypted cookie value
            string cookieValue = _httpContextAccessor.HttpContext.Request.Cookies[".AspNetCore.Identity.Application"];

            if (cookieValue == null || cookieValue.Length == 0)
                return null;

            return cookieValue;
        }
    }

    public class InitialApplicationState
    {
        public string Ticket { get; set; }
    }

    public class ApplicationStateProvider
    {
        public string Ticket { get; set; }
    }

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

        public AuthenticationTicket DecryptIdentityCookie()
        {
            if (_appStateProvider == null || 
                _appStateProvider.Ticket == null ||
                _appStateProvider.Ticket.Length == 0)
                return null;

            //Get the encrypted cookie value
            string cookieValue = _appStateProvider.Ticket;

            //Get a data protector to use with either approach
            var dataProtector = _provider.CreateProtector("Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationMiddleware", "Identity.Application", "v2");

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
