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
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderingContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Order>> GetOrders()
        {
            return await OrderingContext.Orders
                .Include(m => m.Reports)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrders(string order)
        {
            return await OrderingContext.Orders
                .Where(o => o.Name.Contains(order))
                .Include(m => m.Reports)
                .ToListAsync();
        }

        public async Task<Order> GetOrder(int id)
        {
            return await OrderingContext.Orders
                .Include(m => m.Reports)
                .SingleOrDefaultAsync(m => m.OrderId == id);
        }

    }
}
