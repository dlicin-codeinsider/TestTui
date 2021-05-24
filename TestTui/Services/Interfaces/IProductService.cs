using Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductModel>> GetAllProducts();
        Task<ProductModel> AddProduct(ProductModel productModel);
    }
}
