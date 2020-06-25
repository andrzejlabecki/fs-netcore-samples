using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.IdentityModel.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Fs.Business.Extensions;
using Fs.Core.Extensions;
using Fs.Core.Interfaces.Services;
using AutoMapper;

namespace WebAPI
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

            services.AddLogging(config => config.ClearProviders())
                    .AddLogging(config => config.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener));

            services.RegisterServices(SharedConfiguration, AppLoggerFactory);
            services.AddHttpContextAccessor();

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly);

            services.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer1", options =>
                {
                    options.Authority = "https://fs-angular-is4.netpoc.com";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "WebAPI";
                })
                .AddJwtBearer("Bearer2", options =>
                {
                    options.Authority = "https://fs-angular-is4-client.netpoc.com";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "WebAPI";
                })
                .AddJwtBearer("Bearer3", options =>
                {
                    options.Authority = "https://fs-blazor-is4-wasm-client.netpoc.com";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "WebAPI";
                })
                .AddJwtBearer("Bearer4", options =>
                {
                    options.Authority = "https://fs-blazor-is4.netpoc.com";
                    options.RequireHttpsMetadata = false;

                    options.Audience = "WebAPI";
                })
                .AddJwtBearer("Bearer5", options =>
                    {
                        options.Authority = "https://is4.netpoc.com";
                        options.RequireHttpsMetadata = false;

                        options.Audience = "WebAPI";
                    });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("Bearer1", "Bearer2", "Bearer3", "Bearer4", "Bearer5")
                    .Build();
            });

            /*services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("https://angular3.netpoc.com")
                               .AllowAnyHeader();
                    });
            });*/

            services.AddRazorPages();
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
                }

                app.UseCustomExceptionMiddleware();

                app.UseHttpsRedirection();

                app.UseRouting();

                app.UseAuthentication();
                //app.UseIdentityServer();
                app.UseAuthorization();

                //app.UseCors();

                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
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
