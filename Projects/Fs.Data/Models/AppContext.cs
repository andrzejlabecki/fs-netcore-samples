using System;

namespace Fs.Data.Models
{
    public class SchemeContext
    {
        public string AuthScheme { get; set; }
        public string Authority { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ResourceId { get; set; }
        public string AuthorizePath { get; set; }
        public string ResponseType { get; set; }
        public string CodeField { get; set; }
        public string SignInUri { get; set; }
        public string SignOutUri { get; set; }
        public string SignOutCallbackPath { get; set; }
    }

    public sealed class AppContext
    {
        private static AppContext instance = new AppContext();

        public bool IsExternalLogin { get; set; }
        public string RedirectUri { get; set; }
        public SchemeContext[] AuthSchemes { get; set; }

        /*public string AuthScheme { get; set; }
        public string Authority { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ResourceId { get; set; }
        public string AuthorizePath { get; set; }
        public string ResponseType { get; set; }
        public string CodeField { get; set; }
        public string SignInUri { get; set; }
        public string SignOutUri { get; set; }
        public string SignOutCallbackPath { get; set; }*/

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static AppContext()
        {
        }

        private AppContext()
        {
        }

        public static AppContext Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }
    }
}
