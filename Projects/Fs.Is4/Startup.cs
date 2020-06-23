using System.Linq;
using System.Diagnostics;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Fs.Data;
using Fs.Models;
using Fs.Business.Extensions;
using Fs.Core.Extensions;
using Fs.Core.Interfaces.Services;
using AutoMapper;

namespace IdentityServer
{
    public class Startup
    {
        private static ILoggerFactory AppLoggerFactory = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true; //Add this line

            ISharedConfiguration SharedConfiguration = services.RegisterSharedConfiguration();

            string appName = SharedConfiguration.GetValue("Tracing:appName");
            string traceFile = SharedConfiguration.GetTraceFilePath();
            TraceLevel traceLevel = (TraceLevel)System.Enum.Parse(typeof(TraceLevel), SharedConfiguration.GetValue("Tracing:traceLevel"));

            Fs.Core.Trace.Init(appName, traceLevel, traceFile);
            Fs.Core.Trace.Write("ConfigureServices()", "Started", TraceLevel.Info);

            SourceSwitch sourceSwitch = new SourceSwitch("POCTraceSwitch", "Verbose");
            AppLoggerFactory = LoggerFactory.Create(builder => { builder.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener); });

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLoggerFactory(AppLoggerFactory).
                UseSqlServer(SharedConfiguration.GetConnectionString("DefaultConnection")));
            services.AddDbContext<LoggerContext>(options =>
                options.UseLoggerFactory(AppLoggerFactory).
                UseSqlServer(SharedConfiguration.GetConnectionString("LoggerConnection")));
            services.AddDbContext<OrderingContext>(options =>
                options.UseLoggerFactory(AppLoggerFactory).
                UseSqlServer(SharedConfiguration.GetConnectionString("DefaultConnection")));

            services.AddLogging(config => config.ClearProviders())
                    .AddLogging(config => config.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener));

            services.RegisterServices(SharedConfiguration, AppLoggerFactory);
            services.AddHttpContextAccessor();

            /*services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
            {
                options.Clients.Add(new IdentityServer4.Models.Client
                {
                    ClientId = "ClientPOC2",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "WebAPI" },
                    ClientSecrets = { new IdentityServer4.Models.Secret("secret".Sha256()) }
                });
                options.Clients.Add(new IdentityServer4.Models.Client
                {
                    ClientId = "Fs.Blazor.Client",
                    ClientSecrets = { new IdentityServer4.Models.Secret("secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,
                    RequireConsent = false,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },

                    // where to redirect to after login
                    RedirectUris = { "https://fs-blazor-client.netpoc.com/signin-oidc" },

                    // where to redirect to after logout
                    PostLogoutRedirectUris = { "https://fs-blazor-client.netpoc.com/signout-callback-oidc" },

                    AllowedCorsOrigins = { "https://fs-blazor-client.netpoc.com" },
                });
                options.Clients.AddSPA(
                        "Fs.Angular.Is4.Client", spa =>
                        spa.WithRedirectUri("https://fs-angular-is4-client.netpoc.com/signin-oidc")
                           .WithLogoutRedirectUri("https://fs-angular-is4-client.netpoc.com/signout-callback-oidc"));
                options.Clients.AddSPA(
                        "Fs.Blazor.Is4.Wasm.Client", spa =>
                        spa.WithRedirectUri("https://fs-blazor-is4-wasm-client.netpoc.com/signin-oidc")
                           .WithLogoutRedirectUri("https://fs-blazor-is4-wasm-client.netpoc.com/signout-callback-oidc"));
                options.Clients.AddSPA(
                        "Fs.Angular.Client", spa =>
                        spa.WithRedirectUri("https://fs-angular-client.netpoc.com/authentication/login-callback")
                           .WithLogoutRedirectUri("https://fs-angular-client.netpoc.com/authentication/logout-callback")
                           .WithScopes(new string[]
                            {
                                IdentityServerConstants.StandardScopes.OpenId
                            }));
                options.Clients.AddSPA(
                        "Fs.Blazor.Wasm.Client", spa =>
                        spa.WithRedirectUri("https://fs-blazor-wasm-client.netpoc.com/authentication/login-callback")
                           .WithLogoutRedirectUri("https://fs-blazor-wasm-client.netpoc.com/authentication/logout-callback")
                           //.WithoutClientSecrets()
                           .WithScopes(new string[]
                            {
                                IdentityServerConstants.StandardScopes.OpenId,
                                IdentityServerConstants.StandardScopes.Profile
                            }));

                options.Clients["Fs.Angular.Client"].AllowedCorsOrigins.Add("https://fs-angular-client.netpoc.com");
                options.Clients["Fs.Angular.Client"].RedirectUris.Add("https://fs-angular-client.netpoc.com/signin-oidc");

                options.Clients["Fs.Blazor.Wasm.Client"].AllowedCorsOrigins.Add("https://fs-blazor-wasm-client.netpoc.com");
                options.Clients["Fs.Blazor.Wasm.Client"].RedirectUris.Add("https://fs-blazor-wasm-client.netpoc.com/signin-oidc");
            });

            services.AddAuthentication()
                .AddIdentityServerJwt();*/

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(Config.Ids)
                //.AddInMemoryApiResources(Config.Apis)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            builder.AddDeveloperSigningCredential();

            /*services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                    options.ClientId = "<insert here>";
                    options.ClientSecret = "<insert here>";
                })
            .AddOpenIdConnect("oidc", "Demo IdentityServer", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.SaveTokens = true;

                options.Authority = "https://demo.identityserver.io/";
                //options.RequireHttpsMetadata = false;
                options.ClientId = "native.code";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
            });*/

            //services.AddRazorPages();
            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder => {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}