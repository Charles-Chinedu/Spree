using Microsoft.EntityFrameworkCore;
using Spree.Data;
using Spree.Interface;
using Spree.Libraries.Models;
using static Spree.Libraries.Response.CustomResponses;

namespace Spree.Services
{
    public class AdminProductServices(StoringData storingData) : IAdminProduct
    {
        private readonly StoringData _storingData = storingData;

        public async Task<ServiceResponse> AddProductAsync(Product model)
        {
            if (model is null) return null!;
            var (flag, message) = await CheckName(model.Name!);
            if (flag)
            {
                _storingData.Products.Add(model);
                await Commit();
                return new ServiceResponse(true, "Product Added")!;
            }
            return new ServiceResponse(flag, message);
        }

        public async Task<ServiceResponse> EditProductAsync(Product model)
        {
            var products = await _storingData.Products.FindAsync(model.Id);
            if (products is null)
                return null!;
            products.Name = model.Name;
            products.Description = model.Description;
            products.Category = model.Category;
            products.ImageUrl = model.ImageUrl;
            products.Quantity = model.Quantity;
            products.InStock = model.InStock;
            products.Price = model.Price;
            products.Brand = model.Brand;

            await Commit();
            return new ServiceResponse(true, "Edited Successsfully");
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = await _storingData.Products
                .Include(_ => _.Category)
                .ToListAsync();
            return products;
        }

        public async Task<List<Product>> GetInStockAsync(bool inStock)
        {
            var products = await _storingData.Products
                        .Include(_ => _.Category)
                        .Where(_ => _.InStock)
                        .ToListAsync();
            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id) => await _storingData.Products
                        .AsNoTracking()
                        .Include(c => c.Category)
                        .FirstOrDefaultAsync(c => c.Id == id);

        public async Task<ServiceResponse> DeleteProductAsync(int id)
        {
            var products = await _storingData.Products.FindAsync(id);
            if (products is not null)
            {
                _storingData.Products.Remove(products);
                await Commit();
                return new ServiceResponse(true, "Product deleted Successfully");
            }
            return new ServiceResponse(false, "Failed to delete Product");
        }


        private async Task Commit() => await _storingData.SaveChangesAsync();

        private async Task<ServiceResponse> CheckName(string name)
        {
            var product = await _storingData.Products.FirstOrDefaultAsync(x => x.Name!.ToLower()!.Equals(name.ToLower()));
            return product is null ? new ServiceResponse(true, null!) : new ServiceResponse(false, "Prouct already exists");
        }

    }
}
