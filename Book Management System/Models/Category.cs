namespace Book_Management_System.Models
{
    public class Category
    {
        public int CategoryId { get; set; } // 主鍵
        public string CategoryName { get; set; } // 分類名稱

        // 導航屬性
        public virtual ICollection<Book> Books { get; set; } // 與 Books 的一對多關係
    }
}
