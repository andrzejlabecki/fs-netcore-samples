﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Fs.Business.Extensions;

namespace Fs.Blazor.Is4.Wasm.Client.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .CompleteAppConfiguration()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
