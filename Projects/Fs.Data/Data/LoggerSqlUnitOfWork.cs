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
    public class LoggerSqlUnitOfWork : ILoggerUnitOfWork
    {
        private readonly LoggerContext _context;
        private ErrorLogRepository _errorLogRepository;

        public LoggerSqlUnitOfWork(LoggerContext context)
        {
            this._context = context;
        }

        public IErrorLogRepository ErrorLogs => _errorLogRepository = _errorLogRepository ?? new ErrorLogRepository(_context);

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
