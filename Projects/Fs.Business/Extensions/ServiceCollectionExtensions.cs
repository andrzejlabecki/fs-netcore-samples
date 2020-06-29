using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Fs.Core.Interfaces.Services;
using Fs.Business.Services;
using Fs.Data.Extensions;

namespace Fs.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, ISharedConfiguration configuration, ILoggerFactory appLoggerFactory)
        {
            services.RegisterDbContexts(configuration, appLoggerFactory);

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IErrorLogService, ErrorLogService>();

            return services;
        }

        public static IServiceCollection AddJwtBearers(this IServiceCollection services, ISharedConfiguration configuration)
        {
            AuthenticationBuilder builder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            IConfigurationSection section = configuration.GetSection("JwtBearers");
            IEnumerable<IConfigurationSection> bearers = section.GetChildren();

            string[] schemes = new string[bearers.Count()];
            int index = 0;

            foreach (IConfigurationSection bearerSection in bearers)
            {
                schemes[index++] = bearerSection.Key;

                builder.AddJwtBearer(bearerSection.Key, options =>
                {
                    options.Authority = bearerSection.GetValue<string>("Authority");
                    options.RequireHttpsMetadata = bearerSection.GetValue<bool>("HttpsMetadata");

                    options.Audience = bearerSection.GetValue<string>("Audience");
                });
            }

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes(schemes)
                    .Build();
            });

            return services;
        }

        public static IServiceCollection AddOidcProviders(this IServiceCollection services, ISharedConfiguration configuration)
        {
            AuthenticationBuilder builder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies");

            IConfigurationSection section = configuration.GetSection("OidcProviders");
            IEnumerable<IConfigurationSection> providers = section.GetChildren();

            foreach (IConfigurationSection providerSection in providers)
            {
                builder.AddOpenIdConnect(providerSection.Key, options =>
                {
                    options.Authority = providerSection.GetValue<string>("Authority");
                    if (options.Authority == null)
                        options.Authority = configuration.GetOidcLink();

                    options.ClientId = providerSection.GetValue<string>("ClientID");
                    options.ClientSecret = providerSection.GetValue<string>("ClientSecret");
                    options.ResponseType = providerSection.GetValue<string>("ResponseType");
                    options.RequireHttpsMetadata = providerSection.GetValue<bool>("HttpsMetadata");
                    options.SaveTokens = providerSection.GetValue<bool>("SaveTokens");
                    options.GetClaimsFromUserInfoEndpoint = providerSection.GetValue<bool>("ClaimsUserEndpoint");
                    options.Scope.Add(providerSection.GetValue<string>("Scope"));

                    if (providerSection.GetValue<bool>("Events"))
                    {
                        options.Events = new OpenIdConnectEvents
                        {
                            // called if user clicks Cancel during login
                            OnAccessDenied = context =>
                            {
                                context.Response.Redirect("/");
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }
                        };
                    }

                });
            }

            return services;
        }

        public static ISharedConfiguration RegisterSharedConfiguration(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.AddScoped<ISharedConfiguration, SharedConfiguration>().BuildServiceProvider();

            return serviceProvider.GetRequiredService<ISharedConfiguration>();
        }
    }
}
