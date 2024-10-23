using Spree.Libraries.Models;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Interface
{
    public interface ICategory
    {
        Task<ServiceResponse> AddCategory(Category model);

        Task<List<Category>> GetAllCategoriesAsync();

        Task<ServiceResponse> EditCategoryAsync(Category model);

        Task<ServiceResponse> DeleteCategoryAsync(int id);
    }
}