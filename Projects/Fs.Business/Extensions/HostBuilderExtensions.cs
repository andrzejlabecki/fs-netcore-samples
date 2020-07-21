using System;
using System.IO;
using System.Configuration;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Fs.Core.Interfaces.Services;
using Fs.Business.Services;

namespace Fs.Business.Extensions
{
    public static class HostBuilderExtensions
    {
        public static IHostBuilder CompleteAppConfiguration(this IHostBuilder builder, Action<IConfigurationBuilder> action = null)
        {
            builder.ConfigureAppConfiguration((hostingContext, configBuilder) =>
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

                if (action != null)
                    action(configBuilder);
            });

            return builder;
        }

        public static IWebHostBuilder CompleteHostConfiguration(this IWebHostBuilder builder, ISharedConfiguration SharedConfiguration)
        {
            builder.UseSetting(WebHostDefaults.ApplicationKey, SharedConfiguration.GetValue("Hosting:appKey"))
                .CaptureStartupErrors(true)
                .UseSetting(WebHostDefaults.DetailedErrorsKey, "true")
                .UseSetting("https_port", SharedConfiguration.GetValue("Hosting:httpsPort"))
                .PreferHostingUrls(true)
                .UseUrls(SharedConfiguration.GetStringArray("Hosting:useUrls"))
                .UseHttpSys(options =>
                {
                    UrlPrefixCollection urlPrefixesColl = options.UrlPrefixes;
                    AddUrlPrefixes(SharedConfiguration, ref urlPrefixesColl);
                });

            return builder;
        }

        private static void AddUrlPrefixes(ISharedConfiguration configuration, ref UrlPrefixCollection urlPrefixesColl)
        {
            string urlPrefixesValue = configuration.GetValue("Hosting:urlPrefixes");

            if (urlPrefixesValue != null)
            {
                string[] prefixsValues = urlPrefixesValue.Split(" ");

                foreach (string prefix in prefixsValues)
                {
                    urlPrefixesColl.Add(prefix);
                }
            }
        }

        public static IServiceCollection CreateConfigurationBuilder()
        {
            var sharedSettings = Path.GetFullPath(ConfigurationManager.AppSettings["SharedSettings"]);

            var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(sharedSettings, optional: true)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables();

            SharedConfiguration sharedConfiguration = new SharedConfiguration(configBuilder.Build());

            return (new ServiceCollection()).RegisterSharedConfiguration(sharedConfiguration);
        }

        public static IServiceCollection CreateConfigurationBuilder(ServiceCollection services)
        {
            var sharedSettings = Path.GetFullPath(ConfigurationManager.AppSettings["SharedSettings"]);

            var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile(sharedSettings, optional: true)
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddEnvironmentVariables();

            SharedConfiguration sharedConfiguration = new SharedConfiguration(configBuilder.Build());

            return services.RegisterSharedConfiguration(sharedConfiguration);
        }
    }
}
