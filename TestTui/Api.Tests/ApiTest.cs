using Api.Controllers;
using AutoMapper;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services;
using Services.Interfaces;
using Services.Mapper;
using System;
using System.Collections.Generic;
using Xunit;

namespace Api.Tests
{
    public class ApiTest
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

        private ProductController InitController(Context context)
        {
            var mappingConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new ProductMapper());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            IProductRepository repo = new ProductRepository(context);
            IProductService service = new ProductService(mapper, repo);
            ProductController controller = new ProductController(service);

            return controller;
        }

        [Fact]
        public void GetAllProducts_Return_Filled_Products_List()
        {
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

            ProductController controller = InitController(context);

            IActionResult result = controller.GetAllProducts().Result;
            ObjectResult objectResult = result as ObjectResult;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.IsAssignableFrom<IEnumerable<ProductModel>>(objectResult.Value);
            Assert.NotEmpty(objectResult.Value as IEnumerable<ProductModel>);

            DisposeContext(context);
        }

        [Fact]
        public void GetAllProducts_Return_Empty_Products_List()
        {
            Context context = InitContext();

            ProductController controller = InitController(context);

            IActionResult result = controller.GetAllProducts().Result;
            ObjectResult objectResult = result as ObjectResult;

            Assert.Equal(200, objectResult.StatusCode);
            Assert.IsAssignableFrom<IEnumerable<ProductModel>>(objectResult.Value);
            Assert.Empty(objectResult.Value as IEnumerable<ProductModel>);

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
            ProductController controller = InitController(context);

            IActionResult result = controller.PostProduct(product).Result;
            ObjectResult objectResult = result as ObjectResult;

            Assert.Equal(201, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.IsType<ProductModel>(objectResult.Value);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Incorrect_Date_Rule_Return_Error_Message()
        {
            ProductModel product = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 1, 1),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 12, 31)
            };

            Context context = InitContext();
            ProductController controller = InitController(context);

            IActionResult result = controller.PostProduct(product).Result;
            ObjectResult objectResult = result as ObjectResult;

            Assert.Equal(500, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("La date de fin de validit� est ant�rieure � la date de d�but de validit�.", (objectResult.Value as ProblemDetails).Detail);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Code_Duplication_Return_Error_Message()
        {
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

            ProductController controller = InitController(context);

            IActionResult result = controller.PostProduct(productModel).Result;
            ObjectResult objectResult = result as ObjectResult;

            Assert.Equal(500, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal($"Un produit avec le code {product.Code} existe d�j�.", (objectResult.Value as ProblemDetails).Detail);

            DisposeContext(context);
        }

        [Fact]
        public void AddProduct_Null_Input_Return_Error_Message()
        {
            Context context = InitContext();
            ProductController controller = InitController(context);

            IActionResult result = controller.PostProduct(null).Result;
            ObjectResult objectResult = result as ObjectResult;

            Assert.Equal(500, objectResult.StatusCode);
            Assert.NotNull(objectResult.Value);
            Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Le produit a ajout� est null.", (objectResult.Value as ProblemDetails).Detail);

            DisposeContext(context);
        }
    }
}
