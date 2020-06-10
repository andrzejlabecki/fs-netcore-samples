using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AngularPOC.Data.Models;

namespace AngularPOC.Data
{
    public class LoggerContext : DbContext
    {
        public LoggerContext(DbContextOptions<LoggerContext> options) : base(options)
        {
        }

        public DbSet<ErrorLog> ErrorLogs { get; set; }

    }
}
