using System.ComponentModel.DataAnnotations;

namespace Book_Management_System_WebAPI.DTO
{
    public class LoginDTO
    {
        [Required]
        [MaxLength(100)]
        [Display(Name = "Username")]
        public string? Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }

    }
}
