using System.ComponentModel.DataAnnotations;

namespace Spree.Library.Models
{
    public  class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Product Image URL is required")]
        public string ImageUrl { get; set; }

        [Required, Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int Quantity { get; set; }

        public bool InStock { get; set; } = false;

        [Required(ErrorMessage = "Product Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        public string Brand { get; set; }

        // Relationship many to one 
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
    }
}
