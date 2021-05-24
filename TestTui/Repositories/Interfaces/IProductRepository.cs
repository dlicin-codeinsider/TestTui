using Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> AddProduct(Product product);
        Task<Product> GetProductByCode(string code);
    }
}
