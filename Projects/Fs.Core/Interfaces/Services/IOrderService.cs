using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AngularPOC.Core.Contracts;

namespace AngularPOC.Core.Interfaces.Services
{
    public interface IOrderService
    {
        Task<OrderDto> SaveOrder(OrderDto order);
        Task DeleteOrder(OrderDto order);
        Task<IEnumerable<OrderDto>> GetOrders(string order = "");
        Task<OrderDto> GetOrder(int id);
        Task<IEnumerable<ReportDto>> GetReports();
        Task<IEnumerable<ReportDto>> GetReports(int orderId);
        Task<ReportDto> GetReport(int id);
        Task<ReportDto> SaveReport(ReportDto report);
    }
}
