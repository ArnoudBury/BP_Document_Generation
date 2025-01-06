using BP_Document_Generation.Context;
using BP_Document_Generation.Models;
using BP_Document_Generation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BP_Document_Generation.Services {
    public class CustomerService : ICustomerService {

        private readonly ApplicationDBContext _context;

        public CustomerService(ApplicationDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync() {
            return await _context.Customer.ToListAsync();
        }

        public async Task<Customer?> GetCustomerByIdAsync(int customerId) {
            return await _context.Customer.FindAsync(customerId);
        }

        public async Task AddCustomerAsync(Customer customer) {
            await _context.Customer.AddAsync(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCustomerAsync(Customer customer) {
            _context.Customer.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCustomerAsync(int customerId) {
            var customer = await _context.Customer.FindAsync(customerId);
            if (customer != null) {
                _context.Customer.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
    }
}
