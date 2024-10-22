using Spree.Library.Models;
using static Spree.Library.Response.CustomResponses;

namespace Spree.Client.Services
{
    public interface ICategoryService
    {
        Task<ServiceResponse> AddCategory(Category model);

        Task<List<Category>> GetAllCategoriesAsync();

        Task<ServiceResponse> EditCategory(Category model);

        Task<ServiceResponse> DeleteCategoryAsync(int id);
    }
}
