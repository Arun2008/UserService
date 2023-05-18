using System.ComponentModel.DataAnnotations;

namespace UserService.Models.Request
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Name is required")]
        public string? Name { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public Guid RoleId { get; set; }
    }
}
