using System.ComponentModel.DataAnnotations;

namespace Book_Management_System.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
