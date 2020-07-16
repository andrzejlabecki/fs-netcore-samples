using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fs.Blazor.Service.Services;

namespace Fs.Blazor.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    // find the shared folder in the parent folder
                    var sharedSettings = Path.GetFullPath(ConfigurationManager.AppSettings["SharedSettings"]);

                    //load the SharedSettings first, so that appsettings.json overrwrites it
                    config
                        .AddJsonFile(sharedSettings, optional: true)
                        .AddJsonFile("appsettings.json", optional: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

                    config.AddEnvironmentVariables();
                })
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ServiceA>();
                    services.AddHostedService<ServiceB>();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    Startup.WebHostBuilder = webBuilder;

                    webBuilder.UseSetting(WebHostDefaults.ApplicationKey, "Fs.Blazor.Service")
                        .CaptureStartupErrors(true)
                        .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                        .UseSetting("https_port", "5001")
                        .PreferHostingUrls(true)
                        .UseUrls("https://fs-blazor-service.netpoc.com:5001");

                    webBuilder.UseHttpSys(options =>
                    {
                        options.UrlPrefixes.Add("https://127.0.0.1:5001");
                    });

                    webBuilder.UseStartup<Startup>();
                });
    }
}
