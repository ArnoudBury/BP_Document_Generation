using BP_Document_Generation.Context;
using BP_Document_Generation.Models;
using BP_Document_Generation.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BP_Document_Generation.Services {
    public class ProductService : IProductService {
        private readonly ApplicationDBContext _context;

        public ProductService(ApplicationDBContext context) {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync() {
            return await _context.Product.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int productId) {
            return await _context.Product.FindAsync(productId);
        }

        public async Task AddProductAsync(Product product) {
            await _context.Product.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product) {
            _context.Product.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId) {
            var product = await _context.Product.FindAsync(productId);
            if (product != null) {
                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }
        }
    }
}
