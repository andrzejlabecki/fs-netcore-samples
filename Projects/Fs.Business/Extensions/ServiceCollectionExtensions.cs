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
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4;
using IdentityServer4.Models;
using Fs.Core.Interfaces.Services;
using Fs.Business.Services;
using Fs.Data.Extensions;
using Fs.Business.Controllers;
using Microsoft.VisualBasic;

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
            });

            IConfigurationSection identityServer = configuration.GetSection("IdentityServer");
            IEnumerable<IConfigurationSection> sections = identityServer.GetChildren();

            if (sections.Count() > 0)
                builder.AddIdentityServerJwt()
                .AddCookie("Cookies");
            else
                builder.AddCookie("Cookies");

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

                    string secret = providerSection.GetValue<string>("ClientSecret");
                    if (secret != null && secret.Length > 0)
                        options.ClientSecret = secret;

                    string response = providerSection.GetValue<string>("ResponseType");
                    if (response != null && response.Length > 0)
                        options.ResponseType = response;

                    string scope = providerSection.GetValue<string>("Scope");
                    if (scope != null && scope.Length > 0)
                        options.Scope.Add(scope);

                    options.SaveTokens = providerSection.GetValue<bool>("SaveTokens");
                    options.RequireHttpsMetadata = providerSection.GetValue<bool>("HttpsMetadata");
                    options.GetClaimsFromUserInfoEndpoint = providerSection.GetValue<bool>("ClaimsUserEndpoint");

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

        public static void CopyClients(this IServiceCollection services, ClientCollection source, ref ClientCollection destination)
        {
            foreach (IdentityServer4.Models.Client client in source)
            {
                destination.Add(client);
            }
        }

        public static ClientCollection GetClientCollection(this IServiceCollection services, ISharedConfiguration configuration)
        {
            IConfigurationSection section = configuration.GetSection("OidcClients");
            IEnumerable<IConfigurationSection> clients = section.GetChildren();

            ClientCollection clientColl = new ClientCollection();
            bool isSpa = false;

            foreach (IConfigurationSection clientSection in clients)
            {
                isSpa = clientSection.GetValue<bool>("Spa");

                if (isSpa)
                {
                    clientColl.AddSPA(clientSection.Key, spa =>
                        spa.WithLogoutRedirectUri(clientSection.GetValue<string>("LogoutUris")));

                    var scopes = clientSection.GetValue<string>("Scopes");
                    if (scopes != null && scopes.Length > 0)
                        clientColl[clientSection.Key].AllowedScopes = GetStringCollection(clientSection, "Scopes");

                    var origins = clientSection.GetValue<string>("CorsOrigins");
                    if (origins != null && origins.Length > 0)
                        clientColl[clientSection.Key].AllowedCorsOrigins = GetStringCollection(clientSection, "CorsOrigins");

                    clientColl[clientSection.Key].RedirectUris = GetStringCollection(clientSection, "RedirectUris");
                }
                else
                {
                    clientColl.Add(new IdentityServer4.Models.Client
                    {
                        ClientId = clientSection.Key,
                        AllowedGrantTypes = GetStringCollection(clientSection, "GrantTypes"),
                        AllowedScopes = GetStringCollection(clientSection, "Scopes"),
                        ClientSecrets = GetSecretCollection(clientSection),
                        RequireConsent = clientSection.GetValue<bool>("Consent"),
                        RequirePkce = clientSection.GetValue<bool>("Pkce"),
                        RedirectUris = GetStringCollection(clientSection, "RedirectUris"),
                        PostLogoutRedirectUris = GetStringCollection(clientSection, "LogoutUris"),
                        AllowedCorsOrigins = GetStringCollection(clientSection, "CorsOrigins"),
                    });
                }
            }

            return clientColl;
        }

        private static ICollection<IdentityServer4.Models.Secret> GetSecretCollection(IConfigurationSection section)
        {
            string secretsValue = section.GetValue<string>("Secrets");

            List<IdentityServer4.Models.Secret> secrets = new List<IdentityServer4.Models.Secret>();

            if (secretsValue != null)
            {
                string [] secretValues = secretsValue.Split(" ");

                foreach (string secret in secretValues)
                {
                    secrets.Add(new IdentityServer4.Models.Secret(secret.Sha256()));
                }
            }

            return secrets;
        }

        private static ICollection<string> GetStringCollection(IConfigurationSection section, string name)
        {
            string scopes = section.GetValue<string>(name);

            if (scopes != null)
                return scopes.Split(" ");
            else
                return new string[0];
        }

        public static ISharedConfiguration RegisterSharedConfiguration(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.AddSingleton<ISharedConfiguration, SharedConfiguration>().BuildServiceProvider();

            return serviceProvider.GetRequiredService<ISharedConfiguration>();
        }
    }
}
