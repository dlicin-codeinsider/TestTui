using AutoMapper;
using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Interfaces;
using Services.Mapper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Services.Tests
{
    public class ServicesTest
    {
        private Context InitContext()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            Context context = new Context(options);

            return context;
        }

        private void DisposeContext(Context context)
        {
            context.Products.RemoveRange(context.Products);
            context.SaveChanges();
            context.Dispose();
        }

        private IProductService InitService(Context context)
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductMapper());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            IProductRepository repo = new ProductRepository(context);
            IProductService service = new ProductService(mapper, repo);

            return service;
        }

        [Fact]
        public void GetAllProducts_Return_Filled_Products_List()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            Product product1 = new Product()
            {
                Code = "TEST1",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST1",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            Product product2 = new Product()
            {
                Code = "TEST2",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST2",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            Context context = InitContext();

            context.Add(product1);
            context.Add(product2);
            context.SaveChanges();

            IProductService service = InitService(context);

            IEnumerable<ProductModel> result = service.GetAllProducts().Result;

            Assert.NotNull(result);
            Assert.NotEmpty(result);

            DisposeContext(context);
        }

        [Fact]
        public void GetAllProducts_Return_Empty_Products_List()
        {
            Context context = InitContext();
            IProductService service = InitService(context);

            IEnumerable<ProductModel> result = service.GetAllProducts().Result;

            Assert.NotNull(result);
            Assert.Empty(result);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Correct_Input_Save_In_Context()
        {
            ProductModel product = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            Context context = InitContext();
            IProductService service = InitService(context);

            Assert.NotNull(service.AddProduct(product).Result);
            Assert.True(context.Set<Product>().AnyAsync(x => x.Code == "TEST").Result);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Incorrect_Date_Rule_Throws_ProductException()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            ProductModel product = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 1, 1),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 12, 31)
            };

            Context context = InitContext();
            IProductService service = InitService(context);

            ProductException exception = Assert.ThrowsAsync<ProductException>(() => service.AddProduct(product)).Result;
            Assert.Equal("La date de fin de validité est antérieure à la date de début de validité.", exception.Message);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Code_Duplication_Throws_ProductException()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            ProductModel productModel = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            Product product = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            Context context = InitContext();

            context.Add(product);
            context.SaveChanges();

            IProductService service = InitService(context);

            ProductException exception = Assert.ThrowsAsync<ProductException>(() => service.AddProduct(productModel)).Result;
            Assert.Equal($"Un produit avec le code {product.Code} existe déjà.", exception.Message);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Null_Input_Throws_Exception()
        {
            Context context = InitContext();
            IProductService service = InitService(context);

            Assert.ThrowsAsync<Exception>(() => service.AddProduct(null));

            DisposeContext(context);
        }
    }
}
