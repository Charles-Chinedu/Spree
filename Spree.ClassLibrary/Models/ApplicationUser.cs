namespace Spree.Library.Models
{
    public class ApplicationUser
    {
        public int Id { get; set; }

        public string Name { get; set; } 

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }
    }
}
