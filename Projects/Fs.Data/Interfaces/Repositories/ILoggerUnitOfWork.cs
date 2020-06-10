using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngularPOC.Data.Models;

namespace AngularPOC.Data.Interfaces.Repositories
{
    public interface ILoggerUnitOfWork
    {
        IErrorLogRepository ErrorLogs { get; }
        Task<int> CommitAsync();
    }
}
