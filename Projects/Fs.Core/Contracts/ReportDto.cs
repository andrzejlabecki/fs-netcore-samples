using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fs.Core.Contracts
{
    public class ReportDto
    {
        public int ReportId { get; set; }
        public string Name { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int OrderId { get; set; }
        public OrderDto Order { get; set; }
    }
}
