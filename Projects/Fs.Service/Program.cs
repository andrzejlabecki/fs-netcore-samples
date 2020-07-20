using System;
using System.IO;
using System.Configuration;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Fs.Data;
using Fs.Business.Services;
using Fs.Business.Extensions;
using Fs.Core.Interfaces.Services;
using AutoMapper;

namespace Fs.Service
{
    public class Program
    {
        //public static ILoggerFactory ServiceLoggerFactory = null;
        //private static DbContextOptions<OrderingContext> options = null;
        private static ILogger<OrderService> logger = null;
        private static IMapper mapper = null;

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            Fs.Core.Trace.Write("Main()", "Exited", System.Diagnostics.TraceLevel.Info);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, configBuilder) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    // find the shared folder in the parent folder
                    var sharedSettings = Path.GetFullPath(ConfigurationManager.AppSettings["SharedSettings"]);

                    //load the SharedSettings first, so that appsettings.json overrwrites it
                    configBuilder
                        .AddJsonFile(sharedSettings, optional: true)
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                    configBuilder.AddEnvironmentVariables();

                    IConfiguration config = configBuilder.Build();

                    SharedConfiguration sharedConfiguration = new SharedConfiguration(config);

                    var services = new ServiceCollection();

                    ISharedConfiguration SharedConfiguration = services.RegisterSharedConfiguration(sharedConfiguration);
                    services.AddTrace(SharedConfiguration);

                    /*string appName = SharedConfiguration.GetValue("Tracing:appName");
                    string traceFile = SharedConfiguration.GetTraceFilePath();
                    TraceLevel traceLevel = (TraceLevel)System.Enum.Parse(typeof(TraceLevel), SharedConfiguration.GetValue("Tracing:traceLevel"));

                    Fs.Core.Trace.Init(appName, traceLevel, traceFile);
                    Fs.Core.Trace.Write("CreateHostBuilder()", "Started", TraceLevel.Info);

                    SourceSwitch sourceSwitch = new SourceSwitch("POCTraceSwitch", "Verbose");
                    ServiceLoggerFactory = LoggerFactory.Create(builder => { builder.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener); });

                    var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>();
                    options = optionsBuilder
                            .UseLoggerFactory(ServiceLoggerFactory) // Warning: Do not create a new ILoggerFactory instance each time
                            .UseSqlServer(SharedConfiguration.GetConnectionString("DefaultConnection"))
                            .Options;

                    optionsBuilder.EnableSensitiveDataLogging(true);*/

                    services.InitializeOptionsBuilder(SharedConfiguration);

                    services/*.AddLogging(config => config.ClearProviders())
                               .AddLogging(config => config.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener))*/
                               .AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly);

                    IServiceProvider serviceProvider = services.BuildServiceProvider();

                    services.RegisterServices2(SharedConfiguration);

                    logger = serviceProvider.GetRequiredService<ILogger<OrderService>>();
                    mapper = serviceProvider.GetRequiredService<IMapper>();
                    Fs.Core.Trace.Write("CreateHostBuilder()", "Configured", System.Diagnostics.TraceLevel.Info);
                })
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
