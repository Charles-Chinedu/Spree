using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spree.Interface;
using Spree.Libraries.Models;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Controller
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategory categoryService) : ControllerBase
    {
        [HttpGet("All-Categories")]
        public async Task<ActionResult<List<Category>>> GetAllCategoriesAsync()
        {
            var categories = await categoryService.GetAllCategoriesAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound("No categories found."); // Return 404 if no categories are found
            }
            return Ok(categories);
        }

        [HttpPost("Add-Category")]
        public async Task<ActionResult<ServiceResponse>> AddCategory(Category model)
        {
            if (model is null) return BadRequest("Model is null");
            var response = await categoryService.AddCategory(model);
            return new ServiceResponse(true, "Product added Successfully");
        }

        [HttpPut("Edit-Category")]
        public async Task<ActionResult<ServiceResponse>> EditCategory(Category model)
        {
            if (model is null) return new ServiceResponse(false, "No Category found");
            var response = await categoryService.EditCategoryAsync(model);
            return new ServiceResponse(true, "Edited Successfully");
        }

        [HttpDelete("Delete-Category/{id}")]
        public async Task<ActionResult<ServiceResponse>> DeleteCategoryAsync(int id)
        {
            if (id == 0) return new ServiceResponse(false, "No Category found");
            var response = await categoryService.DeleteCategoryAsync(id);
            return new ServiceResponse(true, "Category Deleted");
        }
    }
}
