using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Fs.Business.Extensions;
using Fs.Core.Extensions;
using AutoMapper;

namespace Fs.Blazor.Service
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.RegisterSharedConfiguration();
            services.AddTrace();

            services.RegisterServices(false);

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddOidcProviders(false);

            services.AddServerSideBlazor();
            services.AddSingleton<Fs.Client.Services.ForecastService>();
            services.AddSingleton<Fs.Client.Services.OrderService>();
            services.AddScoped<ApplicationStateProvider>();
            services.AddScoped<AuthenticationStateProvider, BlazorServerAuthState>();
            services.AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly);
        }

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
