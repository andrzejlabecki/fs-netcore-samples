using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Fs.Data;
using Fs.Data.Models;
using Fs.Business.Extensions;
using Fs.Core.Extensions;
using Fs.Core.Interfaces.Services;
using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4;

namespace Fs
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

            ClientCollection clientColl = services.GetClientCollection(SharedConfiguration);

            services.AddIdentityServer()
            .AddApiAuthorization<ApplicationUser, ApplicationDbContext>(options =>
            {
                services.CopyClients(options.Clients, ref clientColl);
            })
            .AddInMemoryClients(clientColl);

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddControllersWithViews();
            services.AddRazorPages();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

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
                if (!env.IsDevelopment())
                {
                    app.UseSpaStaticFiles();
                }

                app.UseRouting();

                app.UseAuthentication();
                app.UseIdentityServer();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                    endpoints.MapRazorPages();
                });

                app.UseSpa(spa =>
                {
                    // To learn more about options for serving an Angular SPA from ASP.NET Core,
                    // see https://go.microsoft.com/fwlink/?linkid=864501

                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment())
                    {
                        spa.UseAngularCliServer(npmScript: "start");
                    }
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
