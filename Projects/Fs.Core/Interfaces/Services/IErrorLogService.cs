using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fs.Core.Contracts;

namespace Fs.Core.Interfaces.Services
{
    public interface IErrorLogService
    {
        Task<ErrorLogDto> SaveErrorLog(ErrorLogDto order);
    }
}
