using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Fs.Data.Repositories;
using Fs.Data.Models;
using Fs.Data.Interfaces.Repositories;

namespace Fs.Data
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
