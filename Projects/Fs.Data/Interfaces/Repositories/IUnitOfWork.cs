using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngularPOC.Data.Models;

namespace AngularPOC.Data.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IOrderRepository Orders { get; }
        IReportRepository Reports { get; }
        Task<int> CommitAsync();
    }
}
