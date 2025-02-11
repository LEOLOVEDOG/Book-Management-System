using System.ComponentModel.DataAnnotations;

namespace Book_Management_System_WebAPI.Models
{
    public class BorrowRecord
    {
        [Key]
        public int BorrowId { get; set; }

        public Guid UserId { get; set; }

        public int BookId { get; set; }

        public DateTime BorrowDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public DateTime? DueDate { get; set; }

        public bool IsReturned { get; set; }

        // 導航屬性
        public virtual User? BorrowingUser { get; set; }
        public virtual Book? BorrowedBook { get; set; }
    }
}
