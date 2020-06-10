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
    public class ReportRepository : RepositoryBase<Report>, IReportRepository
    {
        public ReportRepository(OrderingContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Report>> GetReports()
        {
            return await OrderingContext.Reports
                .Include(m => m.Order)
                /*.Select(r => new Report
                {
                    ReportId = r.ReportId,
                    AddedDate = r.AddedDate,
                    ModifiedDate = r.ModifiedDate,
                    Name = r.Name,
                    OrderId = r.OrderId,
                    Order = r.Order..Select(o => o.Name)
                })*/
                .ToListAsync();
        }

        public async Task<IEnumerable<Report>> GetReports(int orderId)
        {
            return await OrderingContext.Reports
                .Include(m => m.Order)
                .Where(m => m.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<Report> GetReport(int id)
        {
            return await OrderingContext.Reports
                .Include(m => m.Order)
                .SingleOrDefaultAsync(m => m.ReportId == id);
        }

    }
}
