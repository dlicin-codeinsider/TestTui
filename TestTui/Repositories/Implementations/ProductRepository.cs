using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class ProductRepository : IProductRepository
    {
        private readonly Context _context;

        public ProductRepository(Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<Product> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> GetProductByCode(string code)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Code == code);
        }
    }
}
