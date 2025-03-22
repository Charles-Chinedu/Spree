using Microsoft.AspNetCore.Mvc;
using Spree.Interface;
using Spree.Libraries.Models;

namespace Spree.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productService) : ControllerBase
    {
        private readonly IProduct _productService = productService;

        [HttpGet("All-Products")]
        public async Task<ActionResult<List<Product>>> GetAllProductsAsync()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("Single-Product/{id}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            var products = await _productService.GetProductByIdAsync(id);
            return Ok(products);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await _productService.GetProductsByCategory(categoryId);
            return Ok(products);
        }
    }
}
