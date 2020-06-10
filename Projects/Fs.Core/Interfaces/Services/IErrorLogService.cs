using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngularPOC.Core.Contracts;

namespace AngularPOC.Core.Interfaces.Services
{
    public interface IErrorLogService
    {
        Task<ErrorLogDto> SaveErrorLog(ErrorLogDto order);
    }
}
