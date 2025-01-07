using BP_Document_Generation.Context;
using BP_Document_Generation.Models;
using BP_Document_Generation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BP_Document_Generation.Services {
    public class OrderService : IOrderService {
        private readonly ApplicationDBContext _context;

        public OrderService(ApplicationDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() {
            return await _context.Order.ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId) {
            return await _context.Order.FindAsync(orderId);
        }

        public async Task<Order?> GetOrderByCustomerIdAsync(int customerId) {
            return await _context.Order.FirstOrDefaultAsync(o => o.CustomerID == customerId);
        }

        public async Task AddOrderAsync(Order order) {
            await _context.Order.AddAsync(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order) {
            _context.Order.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int orderId) {
            var order = await _context.Order.FindAsync(orderId);
            if (order != null) {
                _context.Order.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }
}
