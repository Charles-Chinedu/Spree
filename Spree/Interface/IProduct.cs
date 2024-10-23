using Spree.Libraries.Models;

namespace Spree.Interface
{
    public interface IProduct
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetProductsByCategory(int categoryId);
        
        Task<Product> GetProductByIdAsync(int id);
    }
}
