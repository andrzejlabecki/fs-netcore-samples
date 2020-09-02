using System;
using Microsoft.AspNetCore.DataProtection;

namespace Fs.Data.Services
{
    public class OIDCDataProtectionProvider : IDataProtectionProvider
    {
        private IDataProtectionProvider _root;

        public OIDCDataProtectionProvider(IDataProtectionProvider root)
        {
            _root = root;
        }

        public IDataProtector CreateProtector(string purpose)
        {
            return _root.CreateProtector(purpose);
        }
    }
}
