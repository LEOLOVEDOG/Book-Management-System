using Microsoft.EntityFrameworkCore;

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
        public DbSet<PasswordReset> PasswordResets { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserSocialLogin> UserSocialLogins { get; set; }

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


            // 配置 BorrowRecord 的主鍵
            modelBuilder.Entity<BorrowRecord>()
                .HasKey(br => br.BorrowId);


            // 配置 PasswordResetRequest 與 User 的多對一關係
            modelBuilder.Entity<PasswordReset>()
                .HasOne(pr => pr.User)
                .WithMany(u => u.PasswordResets)
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


            // 配置 RefreshToken 與 User 的多對一關係
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);


            // 配置 UserSocialLogin 的主鍵
            modelBuilder.Entity<UserSocialLogin>()
                .HasKey(ul => ul.Id);


            // 配置 UserSocialLogin 與 User 的多對一關係
            modelBuilder.Entity<UserSocialLogin>()
                .HasOne(ul => ul.User)
                .WithMany(u => u.SocialLogins)
                .HasForeignKey(ul => ul.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
