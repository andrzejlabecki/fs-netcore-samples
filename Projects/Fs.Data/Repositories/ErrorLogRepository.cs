using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Fs.Data.Models;
using Fs.Data.Interfaces.Repositories;
using Fs.Data;

namespace Fs.Data.Repositories
{
    public class ErrorLogRepository : RepositoryBase<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(LoggerContext context)
            : base(context)
        { }

    }
}
