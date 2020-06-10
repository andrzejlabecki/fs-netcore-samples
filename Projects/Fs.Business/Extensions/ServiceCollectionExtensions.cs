using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AngularPOC.Core.Interfaces.Services;
using AngularPOC.Business.Services;
using AngularPOC.Data.Extensions;

namespace AngularPOC.Business.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, ISharedConfiguration configuration, ILoggerFactory appLoggerFactory)
        {
            services.RegisterDbContexts(configuration, appLoggerFactory);

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IErrorLogService, ErrorLogService>();

            return services;
        }

        public static ISharedConfiguration RegisterSharedConfiguration(this IServiceCollection services)
        {
            IServiceProvider serviceProvider = services.AddScoped<ISharedConfiguration, SharedConfiguration>().BuildServiceProvider();

            return serviceProvider.GetRequiredService<ISharedConfiguration>();
        }
    }
}
