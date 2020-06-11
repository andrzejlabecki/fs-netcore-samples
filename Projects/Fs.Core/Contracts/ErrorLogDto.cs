using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fs.Core.Contracts
{

    public class ErrorLogDto
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string CustomMessage { get; set; }

        public string Page { get; set; }

        public string Stack { get; set; }
    }

}
