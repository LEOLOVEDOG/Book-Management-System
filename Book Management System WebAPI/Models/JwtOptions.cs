using System.ComponentModel.DataAnnotations;

namespace Book_Management_System.Models
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required] 
        public string SignKey { get; set; } = null!;

        [Required] 
        public string Issuer { get; set; } = null!;

        public int ExpireMinutes { get; set; } = 60 * 24; // 1 day
    }
}
