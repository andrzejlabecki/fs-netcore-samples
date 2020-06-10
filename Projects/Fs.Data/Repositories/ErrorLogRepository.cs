using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using AngularPOC.Data.Models;
using AngularPOC.Data.Interfaces.Repositories;
using AngularPOC.Data;

namespace AngularPOC.Data.Repositories
{
    public class ErrorLogRepository : RepositoryBase<ErrorLog>, IErrorLogRepository
    {
        public ErrorLogRepository(LoggerContext context)
            : base(context)
        { }

    }
}
