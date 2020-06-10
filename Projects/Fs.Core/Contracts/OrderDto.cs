using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularPOC.Core.Contracts
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string Name { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<ReportDto> Reports { get; set; } = new List<ReportDto>();
    }
}
