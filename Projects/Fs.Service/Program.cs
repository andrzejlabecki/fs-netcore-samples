using System;
using System.IO;
using System.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Fs.Data;
using Fs.Business.Services;
using Fs.Business.Extensions;
using Fs.Core.Interfaces.Services;
using AutoMapper;

namespace Fs.Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            Fs.Core.Trace.Write("Main()", "Exited", System.Diagnostics.TraceLevel.Info);
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .CompleteAppConfiguration(CompleteConfiguration)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                });

        private static void CompleteConfiguration(IConfigurationBuilder configBuilder)
        {
            (new ServiceCollection()).AddSharedConfiguration(new SharedConfiguration(configBuilder.Build()))
                .AddTrace()
                .SetDbContextOptions()
                .AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly)
                .RegisterAppServices();

            Fs.Core.Trace.Write("CreateHostBuilder()", "Configured", System.Diagnostics.TraceLevel.Info);
        }
    }
}
