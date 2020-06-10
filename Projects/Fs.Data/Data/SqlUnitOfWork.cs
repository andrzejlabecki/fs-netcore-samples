using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using AngularPOC.Data.Repositories;
using AngularPOC.Data.Models;
using AngularPOC.Data.Interfaces.Repositories;

namespace AngularPOC.Data
{
    public class SqlUnitOfWork : IUnitOfWork
    {
        private readonly OrderingContext _context;
        private OrderRepository _orderRepository;
        private ReportRepository _reportRepository;

        public SqlUnitOfWork(OrderingContext context)
        {
            this._context = context;
        }

        public IOrderRepository Orders => _orderRepository = _orderRepository ?? new OrderRepository(_context);

        public IReportRepository Reports => _reportRepository = _reportRepository ?? new ReportRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
