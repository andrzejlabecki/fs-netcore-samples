using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fs.Data.Models;

namespace Fs.Data.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IReportRepository Reports { get; }
        Task<int> CommitAsync();
    }
}
