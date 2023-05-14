using Microsoft.EntityFrameworkCore;
using nebrangu.Models;

namespace nebrangu.Repositories
{
    public class OrdersRepo
    {
        private readonly nebranguContext _context;
        
        public OrdersRepo(nebranguContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAll()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetById(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Category)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Manufacturer)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Season)
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                        .ThenInclude(p => p.Weather)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Order> Create(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> Update(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> Delete(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(p => p.Id == id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public int GetPaymentMethod(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.PaymentMethod)
                .FirstOrDefault(o => o.Id == orderId);
            return order.PaymentMethod.Id;
        }
    }
}
