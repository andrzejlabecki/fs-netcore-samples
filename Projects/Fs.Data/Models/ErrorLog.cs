using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fs.Data.Models
{
    /*
CREATE TABLE [dbo].[ErrorLog] (
    [Id]            int IDENTITY (1, 1) not null,
    [AddDate]       datetime NULL,
    [Type]          varchar(50) null,
    [Message]       ntext null,
    [CustomMessage] ntext null,
    [Page]          varchar(255) null,
    [Stack]         ntext null
)    

ALTER TABLE [dbo].[ErrorLog] ADD  CONSTRAINT [DF_ErrorLog_AddDate]  DEFAULT (getdate()) FOR [AddDate]
GO

     */

    [Table("ErrorLog")]
    public class ErrorLog
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Type { get; set; }

        public string Message { get; set; }

        public string CustomMessage { get; set; }

        public string Page { get; set; }

        public string Stack { get; set; }
    }

}
