using System;

namespace Fs.Data.Models
{
    public sealed class AppContext
    {
        private static AppContext instance = new AppContext();

        public bool IsExternalLogin { get; set; }
        public string AuthScheme { get; set; }

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
