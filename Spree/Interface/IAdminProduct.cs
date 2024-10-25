using Spree.Libraries.DTOs;
using Spree.Libraries.Models;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Interface
{
    public interface IAdminProduct
    {
        Task<ServiceResponse> AddProductAsync(Product model);

        Task<ServiceResponse> EditProductAsync(Product model);

        Task<List<ProductDTO>> GetAllProductsAsync();

        Task<List<Product>> GetInStockAsync(bool inStock);

        Task<Product?> GetProductByIdAsync(int id);

        Task<ServiceResponse> DeleteProductAsync(int id);
    }
}
