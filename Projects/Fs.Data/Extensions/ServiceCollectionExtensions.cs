using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Fs.Data.Interfaces.Repositories;
using Fs.Core.Interfaces.Services;

namespace Fs.Data.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection RegisterDbContexts(this IServiceCollection services, ISharedConfiguration configuration, ILoggerFactory appLoggerFactory)
        {
            services.AddDbContext<LoggerContext>(options =>
                options.UseLoggerFactory(appLoggerFactory).
                UseSqlServer(configuration.GetConnectionString("LoggerConnection")));
            services.AddDbContext<OrderingContext>(options =>
                options.UseLoggerFactory(appLoggerFactory).
                UseSqlServer(configuration.GetConnectionString("OrderConnection")));

            services.AddScoped<IUnitOfWork, SqlUnitOfWork>();
            services.AddScoped<ILoggerUnitOfWork, LoggerSqlUnitOfWork>();


            return services;
        }
    }
}
