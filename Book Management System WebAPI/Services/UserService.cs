using Book_Management_System.Models;
using Book_Management_System_WebAPI.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Book_Management_System.Services
{
    public class UserService
    {
        private readonly BookManagementSystemDbContext _dbContext;
        private readonly JwtService _jwtService;

        public UserService(BookManagementSystemDbContext dbContext ,JwtService jwtService)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
        }

        // 處理註冊邏輯
        public async Task<bool> RegisterUserAsync(string username, string password, string email)
        {
            if (await _dbContext.Users.AnyAsync(u => u.Username == username))
            {
                return false;  
            }

            var user = new User
            {
                Username = username,
                PasswordHash = PasswordHasher.HashPassword(password),
                Email = email
            };

            var defaultRole = await _dbContext.Roles.FirstAsync(r => r.Name == "User"); // 預設角色

            //var defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            //if (defaultRole == null)
            //{
            //    // 如果角色不存在，可以新增
            //    defaultRole = new Role { Name = "Admin" };
            //    _dbContext.Roles.Add(defaultRole);
            //    await _dbContext.SaveChangesAsync();
            //}

            user.Roles.Add(defaultRole);
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync(); return true;
        }

        // 處理登入邏輯
        public async Task<User?> ValidateUserAsync(string username, string password, bool remeberme)
        {
            var user = await _dbContext.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
            {
                return null;  // 驗證失敗，返回 null
            }

            return user;  // 驗證成功，返回用戶
        }

    }
}
