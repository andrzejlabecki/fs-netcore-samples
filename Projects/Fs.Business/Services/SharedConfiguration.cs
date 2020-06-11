using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Fs.Core.Interfaces.Services;

namespace Fs.Business.Services
{
    public class SharedConfiguration : ISharedConfiguration
    {
        private readonly IConfiguration configuration;

        public SharedConfiguration(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            return configuration.GetChildren();
        }

        public IChangeToken GetReloadToken()
        {
            return configuration.GetReloadToken();
        }

        public IConfigurationSection GetSection(string key)
        {
            return configuration.GetSection(key);
        }

        public string this[string key]
        {
            get { return configuration[key]; }
            set { configuration[key] = value; }
        }

        public string GetConnectionString(string name)
        {
            string connString = configuration.GetConnectionString(name);

            if (connString == null || connString.Length == 0)
                connString = GetValue("SharedConnectionStrings:" + name);

            return connString;
        }

        public string GetValue(string name)
        {
            IConfigurationSection section = configuration.GetSection(name);
            return section.Value;
        }
}
}
