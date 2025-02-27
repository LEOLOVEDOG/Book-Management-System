using System.ComponentModel.DataAnnotations;

namespace Book_Management_System_WebAPI.Models
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid(); 

        [MaxLength(100)]
        public string? Username { get; set; } 

        [EmailAddress]
        [MaxLength(256)]
        public string? Email { get; set; }  
        public string? PasswordHash { get; set; }  
        public bool EmailConfirmed { get; set; } = false;  
        public bool IsLocked { get; set; } = false;  
        public int FailedLoginAttempts { get; set; } = 0;
        public bool IsExternalLogin { get; set; } = false;
        public int OverdueCount { get; set; } = 0;

        // 導航屬性
        public virtual ICollection<BorrowRecord> BorrowRecords { get; set; } = new List<BorrowRecord>();  

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>(); 

        public virtual ICollection<PasswordReset> PasswordResets { get; set; } = new List<PasswordReset>();

        public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public virtual ICollection<UserSocialLogin> SocialLogins { get; set; } = new List<UserSocialLogin>();
    }
}
