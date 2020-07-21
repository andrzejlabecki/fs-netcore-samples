using System;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using IdentityServer4.Models;
using Fs.Core.Interfaces.Services;
using Fs.Data;
using Fs.Data.Models;
using Fs.Business.Services;
using Fs.Data.Extensions;
using Fs.Business.Controllers;

namespace Fs.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        private static ILoggerFactory AppLoggerFactory = null;
        private static ISharedConfiguration SharedConfiguration = null;

        public static ISharedConfiguration Configuration(this IServiceCollection services)
        {
            return SharedConfiguration;
        }

        public static IServiceCollection AddTrace(this IServiceCollection services)
        {
            string appName = SharedConfiguration.GetValue("Tracing:appName");
            string traceFile = SharedConfiguration.GetTraceFilePath();
            TraceLevel traceLevel = (TraceLevel)System.Enum.Parse(typeof(TraceLevel), SharedConfiguration.GetValue("Tracing:traceLevel"));

            Fs.Core.Trace.Init(appName, traceLevel, traceFile);
            Fs.Core.Trace.Write("ConfigureServices()", "Started", TraceLevel.Info);

            SourceSwitch sourceSwitch = new SourceSwitch("POCTraceSwitch", "Verbose");
            AppLoggerFactory = LoggerFactory.Create(builder => { builder.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener); });

            services.AddLogging(config => config.ClearProviders())
                    .AddLogging(config => config.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener));

            return services;
        }

        public static IServiceCollection RegisterServices(this IServiceCollection services, bool addIdentity = true)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLoggerFactory(AppLoggerFactory).
                UseSqlServer(SharedConfiguration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<LoggerContext>(options =>
                options.UseLoggerFactory(AppLoggerFactory).
                UseSqlServer(SharedConfiguration.GetConnectionString("LoggerConnection")));

            services.RegisterDbContexts(SharedConfiguration, AppLoggerFactory);

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IErrorLogService, ErrorLogService>();

            services.AddHttpContextAccessor();

            if (addIdentity)
                services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection RegisterAppServices(this IServiceCollection services)
        {
            services.RegisterDbContexts(SharedConfiguration, AppLoggerFactory);

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IErrorLogService, ErrorLogService>();

            return services;
        }

        public static DbContextOptions<OrderingContext> GetDbContextOptions(this IServiceCollection services)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>();
            DbContextOptions<OrderingContext> options = optionsBuilder
                    .UseLoggerFactory(AppLoggerFactory) // Warning: Do not create a new ILoggerFactory instance each time
                    .UseSqlServer(SharedConfiguration.GetConnectionString("DefaultConnection"))
                    .Options;

            optionsBuilder.EnableSensitiveDataLogging(true);

            return options;
        }

        public static IServiceCollection SetDbContextOptions(this IServiceCollection services)
        {
            services.GetDbContextOptions();

            return services;
        }

        public static IServiceCollection ConfigureIdentityServer(this IServiceCollection services)
        {
            ClientCollection clientColl = services.GetClientCollection();

            services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
            {
                services.CopyClients(options.Clients, ref clientColl);
            })
            .AddInMemoryClients(clientColl);

            return services;
        }

        public static IServiceCollection AddJwtBearer(this IServiceCollection services)
        {
            AuthenticationBuilder builder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            IConfigurationSection section = SharedConfiguration.GetSection("JwtBearer");

            builder.AddJwtBearer(options =>
            {
                options.Authority = section.GetValue<string>("Authority");
                if (options.Authority == null)
                    options.Authority = SharedConfiguration.GetOidcLink();

                options.RequireHttpsMetadata = section.GetValue<bool>("HttpsMetadata");

                options.Audience = section.GetValue<string>("Audience");
            });

            return services;
        }

        public static IServiceCollection AddJwtBearers(this IServiceCollection services)
        {
            AuthenticationBuilder builder = services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);

            IConfigurationSection section = SharedConfiguration.GetSection("JwtBearers");
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

        public static IServiceCollection AddOidcProviders(this IServiceCollection services, bool addServerJwt = true)
        {
            AuthenticationBuilder builder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            });


            if (addServerJwt)
                builder.AddIdentityServerJwt();
            builder.AddCookie("Cookies");

            IConfigurationSection section = SharedConfiguration.GetSection("OidcProviders");
            IEnumerable<IConfigurationSection> providers = section.GetChildren();

            foreach (IConfigurationSection providerSection in providers)
            {
                builder.AddOpenIdConnect(providerSection.Key, options =>
                {
                    options.Authority = providerSection.GetValue<string>("Authority");
                    if (options.Authority == null)
                        options.Authority = SharedConfiguration.GetOidcLink();

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

        public static ClientCollection GetClientCollection(this IServiceCollection services)
        {
            IConfigurationSection section = SharedConfiguration.GetSection("OidcClients");
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
                        clientColl[clientSection.Key].AllowedScopes = SharedConfiguration.GetStringCollection(clientSection, "Scopes");

                    var origins = clientSection.GetValue<string>("CorsOrigins");
                    if (origins != null && origins.Length > 0)
                        clientColl[clientSection.Key].AllowedCorsOrigins = SharedConfiguration.GetStringCollection(clientSection, "CorsOrigins");

                    clientColl[clientSection.Key].RedirectUris = SharedConfiguration.GetStringCollection(clientSection, "RedirectUris");
                }
                else
                {
                    clientColl.Add(new IdentityServer4.Models.Client
                    {
                        ClientId = clientSection.Key,
                        AllowedGrantTypes = SharedConfiguration.GetStringCollection(clientSection, "GrantTypes"),
                        AllowedScopes = SharedConfiguration.GetStringCollection(clientSection, "Scopes"),
                        ClientSecrets = GetSecretCollection(clientSection),
                        RequireConsent = clientSection.GetValue<bool>("Consent"),
                        RequirePkce = clientSection.GetValue<bool>("Pkce"),
                        RedirectUris = SharedConfiguration.GetStringCollection(clientSection, "RedirectUris"),
                        PostLogoutRedirectUris = SharedConfiguration.GetStringCollection(clientSection, "LogoutUris"),
                        AllowedCorsOrigins = SharedConfiguration.GetStringCollection(clientSection, "CorsOrigins"),
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

        public static IServiceCollection RegisterSharedConfiguration(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.AddSingleton<ISharedConfiguration, SharedConfiguration>().BuildServiceProvider();

            SharedConfiguration = serviceProvider.GetRequiredService<ISharedConfiguration>();

            return services;
        }

        public static IServiceCollection RegisterSharedConfiguration(this IServiceCollection services, SharedConfiguration sharedConfiguration)
        {
            IServiceProvider serviceProvider = services.AddSingleton<ISharedConfiguration>(sharedConfiguration).BuildServiceProvider();

            SharedConfiguration = serviceProvider.GetRequiredService<ISharedConfiguration>();

            return services;
        }

        public static IServiceCollection AddSharedConfiguration(this IServiceCollection services, SharedConfiguration sharedConfiguration)
        {
            IServiceProvider serviceProvider = services.AddSingleton<ISharedConfiguration>(sharedConfiguration).BuildServiceProvider();

            SharedConfiguration = serviceProvider.GetRequiredService<ISharedConfiguration>();

            return services;
        }
    }
}
