using System.ComponentModel.DataAnnotations;

namespace Book_Management_System.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
