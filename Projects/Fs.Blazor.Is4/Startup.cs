using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Text;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using IdentityServer4;
using IdentityServer4.Models;
using AutoMapper;
using Fs.Data;
using Fs.Business.Extensions;
using Fs.Core.Extensions;
using Fs.Core.Interfaces.Services;
using Fs.Blazor.Is4.Areas.Identity;
using Fs.Blazor.Is4.Data;
using Fs.Data.Models;
using Microsoft.Extensions.Options;

namespace Fs.Blazor.Is4
{
    public class Startup
    {
        private static ILoggerFactory AppLoggerFactory = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
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

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
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
                    //AllowedGrantTypes = GrantTypes.CodeAndClientCredentials,
                    RequireConsent = false,
                    RequirePkce = true,

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "WebAPI"
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
            .AddIdentityServerJwt();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<ApplicationUser>>();
            services.AddScoped<ApplicationStateProvider>();
            services.AddSingleton<WeatherForecastService>();
            services.AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            logger.LogInformation("App start", null);

            try
            {
                app.UseCors(builder => {
                    builder.AllowAnyOrigin();
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                });

                if (env.IsDevelopment())
                {
                    app.UseDeveloperExceptionPage();
                    app.UseDatabaseErrorPage();
                    app.UseCustomExceptionMiddleware();
                }
                else
                {
                    app.UseCustomExceptionMiddleware();
                    app.UseExceptionHandler("/Error");
                    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                    app.UseHsts();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();

                app.UseRouting();

                app.UseAuthentication();
                app.UseIdentityServer();
                app.UseAuthorization();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                    endpoints.MapBlazorHub();
                    endpoints.MapFallbackToPage("/_Host");
                });
            }
            catch (System.Exception ex)
            {
                object[] args = new object[2];

                args[0] = ex.Message;
                args[1] = ex.StackTrace;

                logger.LogError(ex, "Exception: Message {0}, Stack {1}", args);

                throw;
            }
        }
    }
}
