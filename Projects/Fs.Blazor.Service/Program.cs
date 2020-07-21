using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Fs.Blazor.Service.Services;
using Fs.Business.Extensions;
using Fs.Core.Interfaces.Services;
using Fs.Business.Services;

namespace Fs.Blazor.Service
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ISharedConfiguration SharedConfiguration = HostBuilderExtensions.CreateConfigurationBuilder().Configuration();

            IHostBuilder builder = CreateHostBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                 {
                     webBuilder.UseConfiguration(SharedConfiguration)
                        .UseStartup<Startup>()
                        .CompleteHostConfiguration(SharedConfiguration);
                 });

            await builder.Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<ServiceA>();
                    services.AddHostedService<ServiceB>();
                });
    }
}
