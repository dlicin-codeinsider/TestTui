using Api.Controllers;
using Microsoft.AspNetCore.Mvc;
using Models;
using Moq;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Api.Tests
{
    public class ApiTest
    {
        [Fact]
        public void GetAllProducts_Return_Filled_Products_List()
        {
            // Arrange
            IProductService service = Mock.Of<IProductService>();
            Mock.Get(service).Setup(x => x.GetAllProducts()).ReturnsAsync(new List<ProductModel>
            {
                new ProductModel
                {
                    Code = "TEST",
                    EndValidityDate = new DateTime(2021, 12, 31),
                    Id = 1,
                    Name = "TEST",
                    StartValidityDate = new DateTime(2021, 1, 1)
                },
                new ProductModel
                {
                    Code = "TEST2",
                    EndValidityDate = new DateTime(2021, 12, 31),
                    Id = 2,
                    Name = "TEST2",
                    StartValidityDate = new DateTime(2021, 1, 1)
                }
            });

            ProductController controller = new ProductController(service);

            // Act
            IActionResult result = controller.GetAllProducts().Result;

            // Assert
            ObjectResult objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            IEnumerable<ProductModel> objectResultValue = Assert.IsAssignableFrom<IEnumerable<ProductModel>>(objectResult.Value);
            Assert.Equal(2, objectResultValue.Count());
        }

        [Fact]
        public void GetAllProducts_Return_Empty_Products_List()
        {
            // Arrange
            IProductService service = Mock.Of<IProductService>();
            Mock.Get(service).Setup(x => x.GetAllProducts()).ReturnsAsync(new List<ProductModel>());

            ProductController controller = new ProductController(service);

            // Act
            IActionResult result = controller.GetAllProducts().Result;

            // Assert
            ObjectResult objectResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, objectResult.StatusCode);
            IEnumerable<ProductModel> objectResultValue = Assert.IsAssignableFrom<IEnumerable<ProductModel>>(objectResult.Value);
            Assert.Empty(objectResultValue);
        }

        [Fact]
        public void AddProduct_Correct_Input_Return_Product()
        {
            // Arrange
            ProductModel product = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            ProductModel productResult = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Id = 1,
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            IProductService service = Mock.Of<IProductService>();
            Mock.Get(service).Setup(x => x.AddProduct(product)).ReturnsAsync(productResult);

            ProductController controller = new ProductController(service);

            // Act
            IActionResult result = controller.PostProduct(product).Result;

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(201, objectResult.StatusCode);
            ProductModel objectResultValue = Assert.IsType<ProductModel>(objectResult.Value);
            Assert.Equal("TEST", objectResultValue.Code);
        }

        [Fact]
        public void AddProduct_Incorrect_Date_Rule_Return_Error_Message()
        {
            // Arrange
            ProductModel product = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            IProductService service = Mock.Of<IProductService>();
            Mock.Get(service).Setup(x => x.AddProduct(product)).Throws(new ProductException("La date de fin de validité est antérieure à la date de début de validité."));

            ProductController controller = new ProductController(service);

            // Act
            IActionResult result = controller.PostProduct(product).Result;

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            ProblemDetails objectResultValue = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("La date de fin de validité est antérieure à la date de début de validité.", objectResultValue.Detail);
        }

        [Fact]
        public void AddProduct_Code_Duplication_Return_Error_Message()
        {
            // Arrange
            ProductModel product = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            IProductService service = Mock.Of<IProductService>();
            Mock.Get(service).Setup(x => x.AddProduct(product)).Throws(new ProductException($"Un produit avec le code {product.Code} existe déjà."));

            ProductController controller = new ProductController(service);

            // Act
            IActionResult result = controller.PostProduct(product).Result;

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            ProblemDetails objectResultValue = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal($"Un produit avec le code {product.Code} existe déjà.", objectResultValue.Detail);
        }

        [Fact]
        public void AddProduct_Null_Input_Return_Error_Message()
        {
            // Arrange
            IProductService service = Mock.Of<IProductService>();
            ProductController controller = new ProductController(service);

            // Act
            IActionResult result = controller.PostProduct(null).Result;

            // Assert
            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);
            ProblemDetails objectResultValue = Assert.IsType<ProblemDetails>(objectResult.Value);
            Assert.Equal("Le produit a ajouté est null.", objectResultValue.Detail);
        }
    }
}
