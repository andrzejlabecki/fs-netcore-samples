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

        public string GetTraceFilePath(string appName)
        {
            return string.Format("{0}{1}.Trace_[YYYY][MM][DD].txt", GetValue("Tracing:traceFolder"), appName);
        }

        public string GetTraceFilePath()
        {
            return GetTraceFilePath(GetValue("Tracing:appName"));
        }

        public string GetOidcLink(string name)
        {
            return GetValue(name);
        }

        public string GetOidcLink()
        {
            return GetOidcLink("OidcAuthority:httpLink");
        }

        public string GetExternalLogoutUrl(string clientId)
        {
            return GetValue("IdentityServer:SpaClients:" + clientId + ":LogoutUriExt");
        }

        public System.Collections.Generic.IDictionary<string, string> GetClientParameters(string clientId)
        {
            System.Collections.Generic.IDictionary<string, string> clientParameters = null;

            IConfigurationSection clientSection = configuration.GetSection("IdentityServer:SpaClients:"+ clientId);

            if (clientSection != null)
            {
                clientParameters = new Dictionary<string, string>();

                string authority = clientSection.GetValue<string>("Authority");
                if (authority == null)
                    authority = GetOidcLink();

                clientParameters.Add("authority", authority);
                clientParameters.Add("client_id", clientId);
                clientParameters.Add("redirect_uri", clientSection.GetValue<string>("RedirectUri"));
                clientParameters.Add("post_logout_redirect_uri", clientSection.GetValue<string>("LogoutUri"));
                clientParameters.Add("response_type", clientSection.GetValue<string>("ResponseType"));
                clientParameters.Add("scope", clientSection.GetValue<string>("Scope"));
            }

            return clientParameters;
        }

        public ICollection<string> GetStringCollection(IConfigurationSection section, string name)
        {
            string strings = section.GetValue<string>(name);

            if (strings != null)
                return strings.Split(" ");
            else
                return new string[0];
        }

        public ICollection<string> GetStringCollection(string name)
        {
            string strings = GetValue(name);

            if (strings != null)
                return strings.Split(" ");
            else
                return new string[0];
        }

        public string [] GetStringArray(string name)
        {
            string strings = GetValue(name);

            if (strings != null)
                return strings.Split(" ");
            else
                return new string[0];
        }
    }
}
