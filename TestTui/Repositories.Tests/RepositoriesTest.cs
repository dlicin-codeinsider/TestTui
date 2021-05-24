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

            using (Context context = new Context(options))
            {
                context.Add(product1);
                context.Add(product2);
                context.SaveChanges();

                IProductRepository repo = new ProductRepository(context);
                IEnumerable<Product> result = repo.GetAllProducts().Result;

                Assert.NotNull(result);
                Assert.NotEmpty(result);

                context.Products.RemoveRange(context.Products);
                context.SaveChanges();
            }
        }

        [Fact]
        public void GetAllProducts_Return_Empty_Products_List()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            using (Context context = new Context(options))
            {
                IProductRepository repo = new ProductRepository(context);
                IEnumerable<Product> result = repo.GetAllProducts().Result;

                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public void AddProduct_Correct_Input_Save_In_Context()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            Product product = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            using (Context context = new Context(options))
            {
                IProductRepository repo = new ProductRepository(context);

                Assert.NotNull(repo.AddProduct(product).Result);
                Assert.True(context.Set<Product>().AnyAsync(x => x.Code == "TEST").Result);

                context.Products.RemoveRange(context.Products);
                context.SaveChanges();
            }
        }

        [Fact]
        public void AddProduct_Null_Input_Throws_Exception()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            using (Context context = new Context(options))
            {
                IProductRepository repo = new ProductRepository(context);
                Assert.ThrowsAsync<AggregateException>(() => repo.AddProduct(null));
            }
        }

        [Fact]
        public void GetProductByCode_Correct_Code_Input_Return_Product()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            Product product = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            using (Context context = new Context(options))
            {
                context.Add(product);
                context.SaveChanges();

                IProductRepository repo = new ProductRepository(context);
                Product result = repo.GetProductByCode("TEST").Result;

                Assert.NotNull(result);
                Assert.Equal("TEST", result.Code);

                context.Products.RemoveRange(context.Products);
                context.SaveChanges();
            }
        }

        [Fact]
        public void GetProductByCode_Inexisting_Code_Input_Return_Null()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            Product product = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            using (Context context = new Context(options))
            {
                context.Add(product);
                context.SaveChanges();

                IProductRepository repo = new ProductRepository(context);
                Product result = repo.GetProductByCode("TEST2").Result;

                Assert.Null(result);

                context.Products.RemoveRange(context.Products);
                context.SaveChanges();
            }
        }

        [Fact]
        public void GetProductByCode_Null_Input_Return_Null()
        {
            DbContextOptions<Context> options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "ProductDatabase")
                .Options;

            Product product = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            using (Context context = new Context(options))
            {
                context.Add(product);
                context.SaveChanges();

                IProductRepository repo = new ProductRepository(context);
                Product result = repo.GetProductByCode(null).Result;

                Assert.Null(result);

                context.Products.RemoveRange(context.Products);
                context.SaveChanges();
            }
        }
    }
}
