using Microsoft.EntityFrameworkCore;
using Spree.Data;
using Spree.Interface;
using Spree.Library.Models;
using static Spree.Library.Response.CustomResponses;

namespace Spree.Services
{
    public class CategoryService(StoringData storingData) : ICategory
    {
        private readonly StoringData _storingData = storingData;

        public async Task<ServiceResponse> AddCategory(Category model)
        {
            if (model is null) 
                return null!;
            await CheckName(model.Name);

            var newDataAdded = _storingData.Categories.Add(model).Entity;
            await Commit();
            return new ServiceResponse(true, "Category Added Successfully");
        }


        public async Task<List<Category>> GetAllCategoriesAsync() => 
            await _storingData.Categories
                            .AsNoTracking()
                            .ToListAsync();

        public async Task<ServiceResponse> EditCategoryAsync(Category model)
        {
            var categories = await _storingData.Categories.FindAsync(model.Id);
            if (categories is null)
                return null!;
            categories.Name = model.Name;
            categories.Image = model.Image;

            await Commit();
            return new ServiceResponse(true, "Edited Successfully");
        }

        public async Task<ServiceResponse> DeleteCategoryAsync(int id)
        {
            var category = await _storingData.Categories.FindAsync(id);
            if (category == null)
                return new ServiceResponse(false, "Category not found");
            _storingData.Categories.Remove(category);
            await Commit();
            return new ServiceResponse(true, "Category Deleted Successfully");
        }


        private async Task CheckName(string name)
        {
            var product = await _storingData.Products.FirstOrDefaultAsync(x => x.Name!.ToLower()!.Equals(name.ToLower()));
            return;
        }

        private async Task Commit() => await _storingData.SaveChangesAsync();

    }
}
