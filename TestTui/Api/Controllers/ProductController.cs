using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController
        (
            IProductService productService
        )
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                IEnumerable<ProductModel> products = await _productService.GetAllProducts();
                return Ok(products);
            }
            catch (ProductException ex)
            {
                return Problem(ex.Message);
            }
            catch (Exception )
            {
                return Problem("Une Erreur est survenue lors de la récupération de la liste des produits.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostProduct(ProductModel product)
        {
            if (product == null)
            {
                return Problem("Le produit a ajouté est null.");
            }
            try
            {
                product = await _productService.AddProduct(product);
                return StatusCode(201, product);
            }
            catch (ProductException ex)
            {
                return Problem(ex.Message);
            }
            catch (Exception )
            {
                return Problem("Une Erreur est survenue lors de la création d'un produit.");
            }
        }
    }
}
