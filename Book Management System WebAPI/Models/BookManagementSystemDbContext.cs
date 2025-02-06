using Book_Management_System.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Book_Management_System_WebAPI.Models
{
    public class BookManagementSystemDbContext : DbContext
    {
        public BookManagementSystemDbContext(DbContextOptions<BookManagementSystemDbContext> options)
              : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<Role> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 設置 Category 的唯一約束
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.CategoryName)
                .IsUnique();


            //配置 Book 與 Category 的多對多關係
            modelBuilder.Entity<Book>()
                .HasMany(b => b.Categories)
                .WithMany(c => c.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookCategory",
                    j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId").HasConstraintName("FK_BookCategory_CategoryId"),
                    j => j.HasOne<Book>().WithMany().HasForeignKey("BookId").HasConstraintName("FK_BookCategory_BookId"),
                    j =>
                    {
                        j.ToTable("BookCategories");
                    });


            // 配置 BorrowRecord 與 User 的多對一關係
            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.BorrowingUser)
                .WithMany(u => u.BorrowRecords)
                .HasForeignKey(br => br.UserId);


            // 配置 BorrowRecord 與 Book 的多對一關係
            modelBuilder.Entity<BorrowRecord>()
                .HasOne(br => br.BorrowedBook)
                .WithMany()
                .HasForeignKey(br => br.BookId);


            modelBuilder.Entity<BorrowRecord>()
                .HasKey(br => br.BorrowId);


            // 配置 PasswordResetRequest 與 User 的多對一關係
            modelBuilder.Entity<PasswordResetRequest>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PasswordResetRequests)
                .HasForeignKey(pr => pr.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // 配置 User 與 Role 的多對多關係
            modelBuilder.Entity<User>()
                .HasMany(u => u.Roles)
                .WithMany(r => r.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoles",
                    j => j.HasOne<Role>().WithMany().HasForeignKey("RoleId").HasConstraintName("FK_UserRoles_RoleId"),
                    j => j.HasOne<User>().WithMany().HasForeignKey("UserId").HasConstraintName("FK_UserRoles_UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                    }
                );
        }
    }
}
