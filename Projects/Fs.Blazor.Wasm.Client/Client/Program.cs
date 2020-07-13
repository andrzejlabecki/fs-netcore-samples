using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Fs.Blazor.Wasm.Client.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddHttpClient("BlazorWasmApp.AnonymousAPI", client => {
                client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            });

            builder.Services.AddHttpClient("BlazorClient2.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorClient2.ServerAPI"));

            //builder.Services.AddApiAuthorization();

            builder.Services.AddApiAuthorization(options => options.ProviderOptions.ConfigurationEndpoint = "oidc.json");

            /*builder.Services.AddOidcAuthentication(options =>
            {
                //builder.Configuration.Bind("Local", options.ProviderOptions);

                options.ProviderOptions.Authority = "https://fs-mvc-is4.netpoc.com";
                options.ProviderOptions.ClientId = "Fs.Blazor.Wasm.Client";
                options.ProviderOptions.ResponseType = "code";
                options.ProviderOptions.DefaultScopes.Add("openid");
                options.ProviderOptions.DefaultScopes.Add("profile");
                options.ProviderOptions.DefaultScopes.Add("WebAPI");
                options.ProviderOptions.DefaultScopes.Add("BlazorClient2.ServerAPI");
            });*/

            await builder.Build().RunAsync();
        }
    }
}
