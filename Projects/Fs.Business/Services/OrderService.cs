using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AutoMapper;
using AngularPOC.Core.Interfaces.Services;
using AngularPOC.Data.Interfaces.Repositories;
using AngularPOC.Core.Contracts;
using AngularPOC.Data.Models;

namespace AngularPOC.Business.Services
{
    public class OrderService : IOrderService
    {
        private readonly ILogger<OrderService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderService(ILogger<OrderService> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            this._unitOfWork = unitOfWork;
            this._logger = logger;
            this._mapper = mapper;
        }

        #region Orders

        public async Task<OrderDto> SaveOrder(OrderDto order)
        {
            Order orderMap = _mapper.Map<Order>(order);
            await _unitOfWork.Orders.AddAsync(orderMap);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<OrderDto>(orderMap);
        }

        public async Task DeleteOrder(OrderDto order)
        {
            Order orderMap = _mapper.Map<Order>(order);
            _unitOfWork.Orders.Remove(orderMap);
            await _unitOfWork.CommitAsync();
        }

        public async Task<IEnumerable<OrderDto>> GetOrders(string order = "")
        {

            if (String.IsNullOrEmpty(order))
            {
                IEnumerable<Order> collMap = await _unitOfWork.Orders.GetOrders();
                return _mapper.Map<IEnumerable<OrderDto>>(collMap);
            }
            else
            {
                IEnumerable<Order> collMap = await _unitOfWork.Orders.GetOrders(order);
                return _mapper.Map<IEnumerable<OrderDto>>(collMap);
            }
        }


        public async Task<OrderDto> GetOrder(int id)
        {
            Order orderMap = await _unitOfWork.Orders.GetOrder(id);
            return _mapper.Map<OrderDto>(orderMap);
        }

        #endregion Orders

        #region Reports

        public async Task<IEnumerable<ReportDto>> GetReports()
        {
            _logger.LogInformation("GetReports is called.");

            IEnumerable<Report> collMap = await _unitOfWork.Reports.GetReports();
            return _mapper.Map<IEnumerable<ReportDto>>(collMap);
        }

        public async Task<IEnumerable<ReportDto>> GetReports(int orderId)
        {
            IEnumerable<Report> collMap = await _unitOfWork.Reports.GetReports(orderId);
            return _mapper.Map<IEnumerable<ReportDto>>(collMap);
        }

        public async Task<ReportDto> GetReport(int id)
        {
            Report reportMap = await _unitOfWork.Reports.GetReport(id);
            return _mapper.Map<ReportDto>(reportMap);
        }

        public async Task<ReportDto> SaveReport(ReportDto report)
        {
            if (report.Order != null)
            {
                report.OrderId = report.Order.OrderId;
                report.Order = null;
            }
            Report map = _mapper.Map<Report>(report);
            if (map.ReportId > 0)
            {
                map.ModifiedDate = DateTime.Now;
                _unitOfWork.Reports.Update(map, "AddedDate" /*, "OrderId"*/);
                //repo.Update(entity, e => e.Name, e => e.Description);
                //_unitOfWork.Reports.Property(selector).IsModified = true;
            }
            else
                await _unitOfWork.Reports.AddAsync(map);
            //_unitOfWork.Orders.Unchange(map.Order); // Attach existing Order as unchanged entity 
            await _unitOfWork.CommitAsync();
            //_unitOfWork.Reports.DetachAll(); // Detach all changes
            return _mapper.Map<ReportDto>(map);
        }

        /*
public void Update(T entity, params Expression<Func<T, object>>[] properties)
{
    _dbSet.Attach(entity);
    DbEntityEntry<T> entry = _context.Entry(entity);
    foreach (var selector in properties)
    {
        entry.Property(selector).IsModified = true;
    }
}
repo.Update(entity, e => e.Name, e => e.Description);
         */
        #endregion Reports
    }
}
