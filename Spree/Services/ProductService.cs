using Microsoft.EntityFrameworkCore;
using Spree.Data;
using Spree.Interface;
using Spree.Library.Models;

namespace Spree.Services
{
    public class ProductService(StoringData storingData) : IProduct
    {
        private readonly StoringData _storingData = storingData;

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _storingData.Products
                .Include(_ => _.Category)
                .Where(_ => _.InStock)
                .ToListAsync();
            return products;
        }

        public Task<Product> GetProductByIdAsync(int productId) => 
            _storingData.Products.FirstOrDefaultAsync(_ => _.Id == productId)!;


        public async Task<List<Product>> GetProductsByCategory(int categoryId)
        {
            var products = await _storingData.Products
                .Where(_ => _.CategoryId == categoryId)
                .ToListAsync();
            return products;
        }


        private async Task Commit() => await _storingData.SaveChangesAsync();

        private async Task<Product> CheckName(string name)
        {
            var product = await _storingData.Products.FirstOrDefaultAsync(x => x.Name!.ToLower()!.Equals(name.ToLower()));
            return product!;
        }
    }
}
