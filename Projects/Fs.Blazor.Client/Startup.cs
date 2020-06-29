using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Fs.Data;
using Fs.Core.Extensions;
using Fs.Business.Extensions;
using Fs.Core.Interfaces.Services;
using Fs.Blazor.Client.Data;
using Fs.Blazor.Client.Models;

namespace Fs.Blazor.Client
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

            services.AddLogging(config => config.ClearProviders())
                    .AddLogging(config => config.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener));

            services.RegisterServices(SharedConfiguration, AppLoggerFactory);
            services.AddHttpContextAccessor();

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddOidcProviders(SharedConfiguration);


            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();
            services.AddSingleton<OrderService>();
            services.AddScoped<ApplicationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, BlazorServerAuthState>();
            services.AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            try
            {
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
