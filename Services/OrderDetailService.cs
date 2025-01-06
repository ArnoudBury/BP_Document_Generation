using BP_Document_Generation.Context;
using BP_Document_Generation.Models;
using BP_Document_Generation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BP_Document_Generation.Services {
    public class OrderDetailService : IOrderDetailService {
        private readonly ApplicationDBContext _context;

        public OrderDetailService(ApplicationDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<OrderDetail>> GetAllOrderDetailsAsync() {
            return await _context.OrderDetail.ToListAsync();
        }

        public async Task<OrderDetail?> GetOrderDetailByIdAsync(int orderDetailId) {
            return await _context.OrderDetail.FindAsync(orderDetailId);
        }

        public async Task AddOrderDetailAsync(OrderDetail orderDetail) {
            await _context.OrderDetail.AddAsync(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOrderDetailAsync(OrderDetail orderDetail) {
            _context.OrderDetail.Update(orderDetail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteOrderDetailAsync(int orderDetailId) {
            var orderDetail = await _context.OrderDetail.FindAsync(orderDetailId);
            if (orderDetail != null) {
                _context.OrderDetail.Remove(orderDetail);
                await _context.SaveChangesAsync();
            }
        }
    }
}
