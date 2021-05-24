using AutoMapper;
using Data;
using Models;

namespace Services.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductModel>();
            CreateMap<ProductModel, Product>();
        }
    }
}
