using Spree.Library.Models;
using static Spree.Library.Response.CustomResponses;

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