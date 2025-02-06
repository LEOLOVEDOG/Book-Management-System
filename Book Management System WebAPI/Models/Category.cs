using System.ComponentModel.DataAnnotations;

namespace Book_Management_System.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [MaxLength(100)]
        public string? CategoryName { get; set; }

        // 導航屬性
        public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
