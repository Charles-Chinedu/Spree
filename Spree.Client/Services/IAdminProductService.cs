﻿using Spree.Library.Models;
using static Spree.Library.Response.CustomResponses;

namespace Spree.Client.Services
{
    public interface IAdminProductService
    {
        Task<ServiceResponse> AddProductAsync(Product model);

        Task<ServiceResponse> EditProductAsync(Product model);

        Task<List<Product>> GetAllProductsAsync();

        Task<List<Product>> GetInStockAsync(bool inStock);

        Task<Product> GetProductByIdAsync(int id);

        Task<ServiceResponse> DeleteProductAsync(int id);
    }
}
