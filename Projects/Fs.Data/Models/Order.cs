using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AngularPOC.Data.Models
{
    /*
CREATE TABLE [dbo].[Order] (
    [OrderId]       int IDENTITY (1, 1) not null,
    [Name]          varchar(250) not null,
    [AddedDate]     datetime CONSTRAINT [DF_Order_AddedDate] DEFAULT (getdate()) NOT NULL,
    [ModifiedDate]  datetime CONSTRAINT [DF_Order_ModifiedDate] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([OrderId] ASC)
)    

CREATE NONCLUSTERED INDEX [IX_Order_Name]
    ON [dbo].[Order]([Name] ASC);

     */

    [Table("Order")]
    public class Order
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderId { get; set; }
        public string Name { get; set; }
        public DateTime? AddedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public ICollection<Report> Reports { get; set; } = new List<Report>();
    }
}
