using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spree.Interface;
using Spree.Library.Models;
using static Spree.Library.Response.CustomResponses;

namespace Spree.Controller
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminProductController(IAdminProduct adminProduct) : ControllerBase
    {
        [HttpPost("Add-Product")]
        public async Task<ActionResult<ServiceResponse>> AddProductAsync(Product model)
        {
            var products = await adminProduct.AddProductAsync(model);
            return Ok(products);
        }

        [HttpPut("Edit-Product")]
        public async Task<ActionResult<ServiceResponse>> EditProductAsync(Product model)
        {
            if (model is null)
                return BadRequest("Model is null");
            var products = await adminProduct.EditProductAsync(model);
            return Ok(products);
        }

        [HttpGet("All-Products")]
        public async Task<ActionResult<List<Product>>> GetAllProductsAsync()
        {
            var products = await adminProduct.GetAllProductsAsync();
            return Ok(products);
        }

        [HttpGet("In-Stock")]
        public async Task<ActionResult<List<Product>>> GetInStocksAsync(bool inStock)
        {
            var products = await adminProduct.GetInStockAsync(inStock);
            return Ok(products);
        }

        [HttpGet("Single-Product/{id}")]
        public async Task<ActionResult<Product>> GetProductByIdAsync(int id)
        {
            var products = await adminProduct.GetProductByIdAsync(id);
            return Ok(products);

        }

        [HttpDelete("Delete-Product/{id}")]
        public async Task<ActionResult<ServiceResponse>> DeleteProductAsync(int id)
        {
            var response = await adminProduct.DeleteProductAsync(id);
            return Ok(response);
        }
    }
}
