using AutoMapper;
using Data;
using Models;
using Moq;
using Repositories.Interfaces;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Services.Tests
{
    public class ServicesTest
    {
        [Fact]
        public void GetAllProducts_Return_Filled_Products_List()
        {
            // Arrange
            IEnumerable<Product> productList = new List<Product>()
            {
                new Product
                {
                    Code = "TEST",
                    EndValidityDate = new DateTime(2021, 12, 31),
                    Id = 1,
                    Name = "TEST",
                    StartValidityDate = new DateTime(2021, 1, 1)
                },
                new Product
                {
                    Code = "TEST2",
                    EndValidityDate = new DateTime(2021, 12, 31),
                    Id = 2,
                    Name = "TEST2",
                    StartValidityDate = new DateTime(2021, 1, 1)
                }
            };

            IEnumerable<ProductModel> productModelList = new List<ProductModel>()
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
            };

            IProductRepository repo = Mock.Of<IProductRepository>();
            Mock.Get(repo).Setup(x => x.GetAllProducts()).ReturnsAsync(productList);

            IMapper mapper = Mock.Of<IMapper>();
            Mock.Get(mapper).Setup(x => x.Map<IEnumerable<ProductModel>>(productList)).Returns(productModelList);

            IProductService service = new ProductService(mapper, repo);

            // Act
            IEnumerable<ProductModel> result = service.GetAllProducts().Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetAllProducts_Return_Empty_Products_List()
        {
            // Arrange
            IProductRepository repo = Mock.Of<IProductRepository>();
            Mock.Get(repo).Setup(x => x.GetAllProducts()).ReturnsAsync(new List<Product>());

            IMapper mapper = Mock.Of<IMapper>();
            Mock.Get(mapper).Setup(x => x.Map<IEnumerable<ProductModel>>(new List<Product>())).Returns(new List<ProductModel>());

            IProductService service = new ProductService(mapper, repo);

            // Act
            IEnumerable<ProductModel> result = service.GetAllProducts().Result;

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void AddProduct_Correct_Input_Return_Product()
        {
            // Arrange
            ProductModel productModel = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            ProductModel productModelResult = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Id = 1,
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

            Product productResult = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Id = 1,
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            IProductRepository repo = Mock.Of<IProductRepository>();
            Mock.Get(repo).Setup(x => x.AddProduct(product)).ReturnsAsync(productResult);
            Mock.Get(repo).Setup(x => x.GetProductByCode("TEST")).ReturnsAsync((Product)null);

            IMapper mapper = Mock.Of<IMapper>();
            Mock.Get(mapper).Setup(x => x.Map<Product>(productModel)).Returns(product);
            Mock.Get(mapper).Setup(x => x.Map<ProductModel>(productResult)).Returns(productModelResult);

            IProductService service = new ProductService(mapper, repo);

            // Act
            ProductModel result = service.AddProduct(productModel).Result;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("TEST", result.Code);
        }

        [Fact]
        public void AddProduct_Incorrect_Date_Rule_Throws_ProductException()
        {
            // Arrange
            ProductModel productModel = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 1, 1),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 12, 31)
            };

            ProductModel productModelResult = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 1, 1),
                Id = 1,
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 12, 31)
            };

            Product product = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 1, 1),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 12, 31)
            };

            Product productResult = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 1, 1),
                Id = 1,
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 12, 31)
            };

            IProductRepository repo = Mock.Of<IProductRepository>();
            Mock.Get(repo).Setup(x => x.AddProduct(product)).ReturnsAsync(productResult);
            Mock.Get(repo).Setup(x => x.GetProductByCode("TEST")).ReturnsAsync((Product)null);

            IMapper mapper = Mock.Of<IMapper>();
            Mock.Get(mapper).Setup(x => x.Map<Product>(productModel)).Returns(product);
            Mock.Get(mapper).Setup(x => x.Map<ProductModel>(product)).Returns(productModel);

            IProductService service = new ProductService(mapper, repo);

            // Act
            ProductException exception = Assert.ThrowsAsync<ProductException>(() => service.AddProduct(productModel)).Result;

            // Assert
            Assert.Equal("La date de fin de validité est antérieure à la date de début de validité.", exception.Message);
        }

        [Fact]
        public void AddProduct_Code_Duplication_Throws_ProductException()
        {
            // Arrange
            ProductModel productModel = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            ProductModel productModelResult = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Id = 1,
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

            Product productResult = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Id = 1,
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            IProductRepository repo = Mock.Of<IProductRepository>();
            Mock.Get(repo).Setup(x => x.AddProduct(product)).ReturnsAsync(productResult);
            Mock.Get(repo).Setup(x => x.GetProductByCode("TEST")).ReturnsAsync(productResult);

            IMapper mapper = Mock.Of<IMapper>();
            Mock.Get(mapper).Setup(x => x.Map<Product>(productModel)).Returns(product);
            Mock.Get(mapper).Setup(x => x.Map<ProductModel>(productResult)).Returns(productModelResult);

            IProductService service = new ProductService(mapper, repo);

            // Act
            ProductException exception = Assert.ThrowsAsync<ProductException>(() => service.AddProduct(productModel)).Result;

            // Assert
            Assert.Equal($"Un produit avec le code {product.Code} existe déjà.", exception.Message);
        }

        [Fact]
        public void AddProduct_Null_Input_Throws_Exception()
        {
            // Arrange
            ProductModel productModel = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            ProductModel productModelResult = new ProductModel()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Id = 1,
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

            Product productResult = new Product()
            {
                Code = "TEST",
                EndValidityDate = new DateTime(2021, 12, 31),
                Id = 1,
                Name = "TEST",
                StartValidityDate = new DateTime(2021, 1, 1)
            };

            IProductRepository repo = Mock.Of<IProductRepository>();
            Mock.Get(repo).Setup(x => x.AddProduct(product)).ReturnsAsync(productResult);
            Mock.Get(repo).Setup(x => x.GetProductByCode("TEST")).ReturnsAsync((Product)null);

            IMapper mapper = Mock.Of<IMapper>();
            Mock.Get(mapper).Setup(x => x.Map<Product>(productModel)).Returns(product);
            Mock.Get(mapper).Setup(x => x.Map<ProductModel>(productResult)).Returns(productModelResult);

            IProductService service = new ProductService(mapper, repo);

            // Act
            Assert.ThrowsAsync<Exception>(() => service.AddProduct(null));
        }
    }
}
