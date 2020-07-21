using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using AutoMapper;
using Fs.Business.Extensions;
using Fs.Core.Extensions;
using Fs.Core.Interfaces.Services;

namespace Fs.Mvc.Is4
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            IdentityModelEventSource.ShowPII = true; //Add this line
            services.RegisterSharedConfiguration();
            services.AddTrace();

            services.RegisterServices();
            services.ConfigureIdentityServer();

            services.AddAuthentication()
            .AddIdentityServerJwt();

            services.AddControllersWithViews();
            services.AddRazorPages();
            services.AddServerSideBlazor();
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
                	app.UseExceptionHandler("/Home/Error");
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
	                endpoints.MapControllerRoute(
	                    name: "default",
	                    pattern: "{controller=Home}/{action=Index}/{id?}");
	                endpoints.MapRazorPages();
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
