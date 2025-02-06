using System.ComponentModel.DataAnnotations;

namespace Book_Management_System.Models
{
    public class PasswordResetRequest
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

        [MaxLength(256)]
        public string? ResetToken { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime ExpiryTime { get; set; }

        public bool IsUsed { get; set; } = false;

        // 導航屬性
        public virtual User? User { get; set; }
    }
}
