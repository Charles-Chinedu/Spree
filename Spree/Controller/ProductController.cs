using Microsoft.AspNetCore.Mvc;
using Spree.Interface;
using Spree.Libraries.Models;

namespace Spree.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProduct productService) : ControllerBase
    {
        [HttpGet("All-Products")]
        public async Task<ActionResult<List<Product>>> GetAllProductsAsync()
        {
            var products = await productService.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("Single-Product/{producttId}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            var products = await productService.GetProductByIdAsync(id);
            return Ok(products);
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetProductsByCategory(int categoryId)
        {
            var products = await productService.GetProductsByCategory(categoryId);
            return Ok(products);
        }
    }
}
