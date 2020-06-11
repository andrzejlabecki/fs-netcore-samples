using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Fs.Data.Models;

namespace Fs.Data
{
    public class LoggerContext : DbContext
    {
        public LoggerContext(DbContextOptions<LoggerContext> options) : base(options)
        {
        }

        public DbSet<ErrorLog> ErrorLogs { get; set; }

    }
}
