using System;
using System.Collections.Generic;
using System.Text;

namespace Fs.Models
{
    public class OidcContext
    {
        public bool IsExternalLogin { get; set; }
        public string Authority { get; set; }
    }
}
