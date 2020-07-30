using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fs.Blazor
{
    public class InitialApplicationState
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public string IdentityCookie { get; set; }
    }

    public class ApplicationStateProvider
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset Expiration { get; set; }
        public string IdentityCookie { get; set; }
    }

    public class BlazorServerAuthState 
        : RevalidatingServerAuthenticationStateProvider
    {
        private readonly ApplicationStateProvider StateProvider;

        public BlazorServerAuthState(
            ILoggerFactory loggerFactory,
            ApplicationStateProvider stateProvider)
            : base(loggerFactory)
        {
            //Cache = cache;
            StateProvider = stateProvider;
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
                System.Diagnostics.Debug.WriteLine($"ExpUtc: {StateProvider.Expiration.ToString("o")}");

                if(DateTimeOffset.UtcNow >= StateProvider.Expiration)
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
