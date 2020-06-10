using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AngularPOC.Data.Models;

namespace AngularPOC.Data.Interfaces.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrders();
        Task<IEnumerable<Order>> GetOrders(string order);
        Task<Order> GetOrder(int id);
    }
}
