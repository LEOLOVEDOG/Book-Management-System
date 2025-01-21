namespace Book_Management_System.Models
{
    public class BorrowRecord
    {
        public int BorrowId { get; set; } // 主鍵
        public string UserId { get; set; } // 外鍵，指向 Users 表
        public int BookId { get; set; } // 外鍵，指向 Books 表
        public DateTime BorrowDate { get; set; } // 借閱日期
        public DateTime? ReturnDate { get; set; } // 歸還日期 (可為空)
        public DateTime? DueDate { get; set; } // 應還日期

        // 導航屬性
        public virtual User User { get; set; } // 與 Users 的多對一關係
        public virtual Book Book { get; set; } // 與 Books 的多對一關係
    }
}
