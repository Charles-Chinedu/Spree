using Spree.Library.Models;

namespace Spree.Client.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetProductsByCategory(int categoryId);

        Task<Product> GetProductById(int productId);
    }
}

