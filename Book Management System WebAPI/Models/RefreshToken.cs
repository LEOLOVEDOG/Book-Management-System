using System.ComponentModel.DataAnnotations;

namespace Book_Management_System_WebAPI.Models
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string? JwtId { get; set; }

        [Required]
        [StringLength(256)]
        public string? Token { get; set; }

        /// <summary>
        /// 是否使用，一個RefreshToken只能使用一次
        /// </summary>
        [Required]
        public bool Used { get; set; }

        /// <summary>
        /// 是否失效。修改使用者重要資訊時可將此欄位更新為true，使使用者重新登入
        /// </summary>
        [Required]
        public bool Invalidated { get; set; }

        [Required]
        public DateTime CreationTime { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }

        [Required]
        public Guid UserId { get; set; }

        // 導航屬性
        public virtual User? User { get; set; }


    }
}
