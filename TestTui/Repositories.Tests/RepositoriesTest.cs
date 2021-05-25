using Data;
using Microsoft.EntityFrameworkCore;
using Repositories.Implementations;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using Xunit;

namespace Repositories.Tests
{
    public class RepositoriesTest
    {
        private static Context InitContext()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            Context context = new Context(options);

            return context;
        }

        private static void DisposeContext(Context context)
        {
            context.Products.RemoveRange(context.Products);
            context.SaveChanges();
            context.Dispose();
        }

        [Fact]
        public void GetAllProducts_Return_Filled_Products_List()
        {
            // Arrange
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

            IProductRepository repo = new ProductRepository(context);
            
            // Act
            IEnumerable<Product> result = repo.GetAllProducts().Result;

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);

            DisposeContext(context);
        }

        [Fact]
        public void GetAllProducts_Return_Empty_Products_List()
        {
            // Arrange
            Context context = InitContext();

            IProductRepository repo = new ProductRepository(context);
            
            // Act
            IEnumerable<Product> result = repo.GetAllProducts().Result;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Correct_Input_Save_In_Context()
        {
            // Arrange
            Product product = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            Context context = InitContext();
            
            // Act
            IProductRepository repo = new ProductRepository(context);

            // Assert
            Assert.NotNull(repo.AddProduct(product).Result);
            Assert.True(context.Set<Product>().AnyAsync(x => x.Code == "TEST").Result);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Null_Input_Throws_Exception()
        {
            // Arrange
            Context context = InitContext();

            IProductRepository repo = new ProductRepository(context);

            // Act
            Assert.ThrowsAsync<AggregateException>(() => repo.AddProduct(null));

            DisposeContext(context);
        }

        [Fact]
        public void GetProductByCode_Correct_Code_Input_Return_Product()
        {
            // Arrange
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

            IProductRepository repo = new ProductRepository(context);
            
            // Act
            Product result = repo.GetProductByCode("TEST").Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST", result.Code);

            DisposeContext(context);
        }

        [Fact]
        public void GetProductByCode_Inexisting_Code_Input_Return_Null()
        {
            // Arrange
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

            IProductRepository repo = new ProductRepository(context);
            
            // Act
            Product result = repo.GetProductByCode("TEST2").Result;

            // Assert
            Assert.Null(result);

            DisposeContext(context);
        }

        [Fact]
        public void GetProductByCode_Null_Input_Return_Null()
        {
            // Arrange
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

            IProductRepository repo = new ProductRepository(context);
            
            // Act
            Product result = repo.GetProductByCode(null).Result;

            // Assert
            Assert.Null(result);

            DisposeContext(context);
        }
    }
}
