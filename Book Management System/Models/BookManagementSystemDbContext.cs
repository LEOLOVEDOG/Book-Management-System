using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Book_Management_System.Models
{
    public class BookManagementSystemDbContext : IdentityDbContext<User>
    {
        public BookManagementSystemDbContext(DbContextOptions<BookManagementSystemDbContext> options)
       : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BorrowRecord> BorrowRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          
            base.OnModelCreating(modelBuilder);

            // 設置 Category 的唯一約束
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();

            // 配置 Book 與 Category 的多對一關係
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryId);

            // 配置 BorrowRecord 與 User 的多對一關係
            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.User)
                .WithMany()
                .HasForeignKey(br => br.UserId);

            // 配置 BorrowRecord 與 Book 的多對一關係
            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.Book)
                .WithMany()
                .HasForeignKey(br => br.BookId);

            modelBuilder.Entity<BorrowRecord>()
                .HasKey(br => br.BorrowId); // 配置 BorrowId 為主鍵
        }
    }
}
