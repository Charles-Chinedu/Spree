using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Spree.Libraries.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, DisplayName("Category Image")]
        public string Image { get; set; }

        // this helps to create a copy of the current object
        public Category Clone() => (MemberwiseClone() as Category)!;

        [JsonIgnore]
        // Relationship one to many
        public List<Product>? Products { get; set; }
    }
}
