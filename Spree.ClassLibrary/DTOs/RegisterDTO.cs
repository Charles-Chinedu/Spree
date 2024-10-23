using System.ComponentModel.DataAnnotations;

namespace Spree.Library.Response
{
    public class RegisterDTO : LoginDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, Compare(nameof(Password)), DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
