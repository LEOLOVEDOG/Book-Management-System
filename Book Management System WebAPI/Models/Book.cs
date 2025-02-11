using System.ComponentModel.DataAnnotations;

namespace Book_Management_System_WebAPI.Models
{
    public class Book
    {
        [Key]
        public int BookId { get; set; }

        [MaxLength(200)]
        public string? Title { get; set; }

        [MaxLength(100)]
        public string? Author { get; set; }

        [MaxLength(20)]
        public string? ISBN { get; set; }

        public int CategoryId { get; set; }

        public int StockCount { get; set; }

        public DateTime PublishedDate { get; set; }

        public bool IsAvailable { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public byte[]? Image { get; set; }

        public int BorrowCount { get; set; }

        // 導航屬性
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
