using System;
using Fs.Blazor.Is4.Wasm.Client.Server.Data;
using Fs.Data.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(Fs.Blazor.Is4.Wasm.Client.Server.Areas.Identity.IdentityHostingStartup))]
namespace Fs.Blazor.Is4.Wasm.Client.Server.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}