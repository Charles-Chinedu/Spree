using Microsoft.EntityFrameworkCore;
using Spree.Library.Models;

namespace Spree.Data
{
    public class StoringData(DbContextOptions options) : DbContext(options)
    {
        public DbSet<Product> Products { get; set; } = default!;
        public DbSet<Category> Categories { get; set; } = default!;
        public DbSet<ApplicationUser> Users { get; set; } = default!;
    }
}
