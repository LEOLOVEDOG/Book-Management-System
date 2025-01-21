namespace Book_Management_System.Models
{
    public class Book
    {
        public int BookId { get; set; } // 主鍵
        public string Title { get; set; } // 書名
        public string Author { get; set; } // 作者
        public string ISBN { get; set; } // 國際標準書號
        public int CategoryId { get; set; } // 外鍵，指向 Categories 表
        public int StockCount { get; set; } // 庫存數量
        public DateTime PublishedDate { get; set; } // 發行日期
        public bool IsAvailable { get; set; } // 是否可借閱

        public string Description { get; set; } // 書本介紹
        public string Image { get; set; } // 圖片
        public int BorrowCount { get; set; } // 借閱次數
        // 導航屬性
        public virtual Category Category { get; set; } // 與 Category 的多對一關係
    }
}
