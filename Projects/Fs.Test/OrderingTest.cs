using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.Extensions.Logging.TraceSource;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Fs.Data;
using Fs.Data.Interfaces.Repositories;
using Fs.Core.Contracts;
using Fs.Core.Interfaces.Services;
using Fs.Business.Services;
using Fs.Business.Extensions;
using AutoMapper;

namespace Fs.Test
{
    public class OrderingTests
    {
        public static ILoggerFactory TestLoggerFactory = null;
        private static DbContextOptions<OrderingContext> options = null;
        private static ILogger<OrderService> logger = null;
        private static IMapper mapper = null;

        [OneTimeSetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            ISharedConfiguration SharedConfiguration = HostBuilderExtensions.CreateConfigurationBuilder(services);

            string appName = SharedConfiguration.GetValue("Tracing:appName");
            string traceFile = SharedConfiguration.GetTraceFilePath();
            TraceLevel traceLevel = (TraceLevel)System.Enum.Parse(typeof(TraceLevel), SharedConfiguration.GetValue("Tracing:traceLevel"));

            Fs.Core.Trace.Init(appName, traceLevel, traceFile);
            Fs.Core.Trace.Write("ConfigureServices()", "Started", TraceLevel.Info);

            SourceSwitch sourceSwitch = new SourceSwitch("POCTraceSwitch", "Verbose");
            TestLoggerFactory = LoggerFactory.Create(builder => { builder.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener); });

            var optionsBuilder = new DbContextOptionsBuilder<OrderingContext>();
            options = optionsBuilder
                    .UseLoggerFactory(TestLoggerFactory) // Warning: Do not create a new ILoggerFactory instance each time
                    .UseSqlServer(SharedConfiguration.GetConnectionString("DefaultConnection"))
                    .Options;

            optionsBuilder.EnableSensitiveDataLogging(true);

            services.AddLogging(config => config.ClearProviders())
                       .AddLogging(config => config.AddTraceSource(sourceSwitch, Fs.Core.Trace.TraceListener))
                       .AddAutoMapper(typeof(Fs.Business.Mappings.MappingProfile).Assembly);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            services.RegisterServices(SharedConfiguration, TestLoggerFactory);

            logger = serviceProvider.GetRequiredService<ILogger<OrderService>>();
            mapper = serviceProvider.GetRequiredService<IMapper>();
            Fs.Core.Trace.Write("Setup()", "Completed", System.Diagnostics.TraceLevel.Info);
        }

        [OneTimeTearDown]
        public void Cleanup()
        {
            Fs.Core.Trace.Write("Cleanup()", "Completed", System.Diagnostics.TraceLevel.Info);
        }

        private static IOrderService GetOrderService(OrderingContext db)
        {
            IUnitOfWork unitOfWork = new SqlUnitOfWork(db);
            return new OrderService(logger, unitOfWork, mapper);
        }

        [Test]
        public async Task GetOrders()
        {
            Fs.Core.Trace.Write("GetOrders()", "Started", System.Diagnostics.TraceLevel.Info);

            using (OrderingContext db = new OrderingContext(options))
            {
                IOrderService service = GetOrderService(db);

                IEnumerable<OrderDto> orders = await service.GetOrders();

                int cnt = ((ICollection<OrderDto>)orders).Count();
            }

            Fs.Core.Trace.Write("GetOrders()", "Completed", System.Diagnostics.TraceLevel.Info);
        }

        [Test]
        public async Task GetReports()
        {
            Fs.Core.Trace.Write("GetReports()", "Started", System.Diagnostics.TraceLevel.Info);

            using (OrderingContext db = new OrderingContext(options))
            {
                IOrderService service = GetOrderService(db);

                IEnumerable<ReportDto> reports = await service.GetReports();

                int cnt = ((ICollection<ReportDto>)reports).Count();
            }

            Fs.Core.Trace.Write("GetReports()", "Completed", System.Diagnostics.TraceLevel.Info);
        }

        [Test]
        public async Task AddOrder()
        {
            Fs.Core.Trace.Write("AddOrder()", "Started", System.Diagnostics.TraceLevel.Info);

            using (OrderingContext db = new OrderingContext(options))
            {
                IOrderService service = GetOrderService(db);

                OrderDto order = new OrderDto()
                {
                    Name = "Order #2"
                };

                ReportDto report = new ReportDto()
                {
                    Name = "Order #2 Report #1",
                    Order = order
                };
                order.Reports.Add(report);

                report = new ReportDto()
                {
                    Name = "Order #2 Report #2",
                    Order = order
                };
                order.Reports.Add(report);

                OrderDto newOrder = await service.SaveOrder(order);
            }

            Fs.Core.Trace.Write("AddOrder()", "Completed", System.Diagnostics.TraceLevel.Info);
        }

        [Test]
        public async Task DeleteOrder()
        {
            Fs.Core.Trace.Write("DeleteOrder()", "Started", System.Diagnostics.TraceLevel.Info);

            try
            {
                OrderDto newOrder = null;

                using (OrderingContext db = new OrderingContext(options))
                {
                    IOrderService service = GetOrderService(db);

                    OrderDto order = new OrderDto()
                    {
                        Name = "Order #2"
                    };

                    ReportDto report = new ReportDto()
                    {
                        Name = "Order #2 Report #1",
                        Order = order
                    };
                    order.Reports.Add(report);

                    report = new ReportDto()
                    {
                        Name = "Order #2 Report #2",
                        Order = order
                    };
                    order.Reports.Add(report);

                    newOrder = await service.SaveOrder(order);
                }

                using (OrderingContext db = new OrderingContext(options))
                {
                    IOrderService service = GetOrderService(db);

                    await service.DeleteOrder(newOrder);
                    OrderDto deletedOrder = await service.GetOrder(newOrder.OrderId);
                    if (deletedOrder != null)
                        throw new System.Exception(string.Format("Order #{0} is not deleted", newOrder.OrderId.ToString()));
                }
            }
            catch (System.Exception ex)
            {
                string errorMessage = "Exception:\r\n";

                errorMessage += "Message: " + ex.Message + "\r\n";
                errorMessage += "Stack: " + ex.StackTrace + "\r\n";
                if (ex.InnerException != null)
                {
                    errorMessage = "Inner Exception:\r\n";
                    errorMessage += "Message: " + ex.InnerException.Message + "\r\n";
                    errorMessage += "Stack: " + ex.InnerException.StackTrace + "\r\n";
                }

                Fs.Core.Trace.Write(ex.Source, errorMessage, System.Diagnostics.TraceLevel.Error);
                throw;
            }

            Fs.Core.Trace.Write("DeleteOrder()", "Completed", System.Diagnostics.TraceLevel.Info);
        }
    }
}