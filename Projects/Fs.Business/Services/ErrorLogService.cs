using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Fs.Core.Interfaces.Services;
using Fs.Data.Interfaces.Repositories;
using Fs.Core.Contracts;
using Fs.Data.Models;

namespace Fs.Business.Services
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly ILoggerUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ErrorLogService(ILogger<OrderService> logger, ILoggerUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
            this._mapper = mapper;
        }

        #region Error logs

        public async Task<ErrorLogDto> SaveErrorLog(ErrorLogDto errorLog)
        {
            ErrorLog map = _mapper.Map<ErrorLog>(errorLog);
            await _unitOfWork.ErrorLogs.AddAsync(map);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<ErrorLogDto>(map);
        }

        #endregion Error logs

    }
}
