using AutoMapper;
using Data;
using Models;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productReposoitory;

        public ProductService
        (
            IMapper mapper,
            IProductRepository productRepository
        )
        {
            _mapper = mapper;
            _productReposoitory = productRepository;
        }

        public async Task<IEnumerable<ProductModel>> GetAllProducts()
        {
            IEnumerable<Product> products = await _productReposoitory.GetAllProducts();
            return _mapper.Map<IEnumerable<ProductModel>>(products);
        }

        public async Task<ProductModel> AddProduct(ProductModel productModel)
        {
            if (productModel.StartValidityDate > productModel.EndValidityDate)
            {
                throw new ProductException("La date de fin de validité est antérieure à la date de début de validité.");
            }

            Product product = await _productReposoitory.GetProductByCode(productModel.Code);
            if (product != null)
            {
                throw new ProductException($"Un produit avec le code {productModel.Code} existe déjà.");
            }

            product = _mapper.Map<Product>(productModel);
            Product productCreated = await _productReposoitory.AddProduct(product);
            return _mapper.Map<ProductModel>(productCreated);
        }
    }
}
