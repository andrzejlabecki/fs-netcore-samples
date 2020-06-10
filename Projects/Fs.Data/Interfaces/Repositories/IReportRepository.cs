using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngularPOC.Data.Models;

namespace AngularPOC.Data.Interfaces.Repositories
{
    public interface IReportRepository : IRepository<Report>
    {
        Task<IEnumerable<Report>> GetReports();
        Task<IEnumerable<Report>> GetReports(int orderId);
        Task<Report> GetReport(int id);
    }
}
