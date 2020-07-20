using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Fs.Core.Contracts;

namespace Fs.Core.Interfaces.Services
{
    public interface ISharedConfiguration : IConfiguration
    {
        string GetConnectionString(string name);
        string GetValue(string name);
        string GetTraceFilePath();
        string GetTraceFilePath(string appName);
        string GetOidcLink();
        string GetOidcLink(string authName);
        System.Collections.Generic.IDictionary<string, string> GetClientParameters(string clientId);
        string GetExternalLogoutUrl(string clientId);
        ICollection<string> GetStringCollection(IConfigurationSection section, string name);
        ICollection<string> GetStringCollection(string name);
        string [] GetStringArray(string name);
    }
}

