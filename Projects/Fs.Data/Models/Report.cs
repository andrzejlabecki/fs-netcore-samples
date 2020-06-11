using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fs.Data.Models
{
    /*
CREATE TABLE [dbo].[Report] (
    [ReportId]      int IDENTITY (1, 1) not null,
    [OrderId]       int not null,
    [Name]          varchar(250) not null,
    [AddedDate]     datetime CONSTRAINT [DF_Report_AddedDate] DEFAULT (getdate()) NOT NULL,
    [ModifiedDate]  datetime CONSTRAINT [DF_Report_ModifiedDate] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Report] PRIMARY KEY CLUSTERED ([ReportId] ASC)
)    

CREATE NONCLUSTERED INDEX [IX_Report_Name]
    ON [dbo].[Report]([Name] ASC);

CREATE NONCLUSTERED INDEX [IX_Report_OrderId]
    ON [dbo].[Report]([OrderId] ASC);

     */

    [Table("Report")]
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportId { get; set; }
        public string Name { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
    }
}
