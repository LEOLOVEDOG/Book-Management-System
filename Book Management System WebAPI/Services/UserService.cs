using Book_Management_System_WebAPI.Interfaces;
using Book_Management_System_WebAPI.Models;
using Book_Management_System_WebAPI.Results;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;

namespace Book_Management_System_WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly BookManagementSystemDbContext _dbContext;
        private readonly IJwtService _jwtService;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        public UserService(BookManagementSystemDbContext dbContext, IJwtService jwtService, IEmailSender emailSender, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _jwtService = jwtService;
            _emailSender = emailSender;
            _configuration = configuration;
        }

        // 處理註冊邏輯
        public async Task<TokenResult> RegisterAsync(string username, string password, string email)
        {
            try
            {
                // 檢查 Username 是否已存在
                if (await _dbContext.Users.AnyAsync(u => u.Username == username))
                {
                    return new TokenResult { Errors = new[] { "Username is already taken." } };
                }

                // 檢查 Email 是否已被使用
                if (await _dbContext.Users.AnyAsync(u => u.Email == email))
                {
                    return new TokenResult { Errors = new[] { "Email is already registered." } };
                }

                var user = new User
                {
                    Username = username,
                    PasswordHash = PasswordHasher.HashPassword(password),
                    Email = email,
                };

                // 取得預設角色
                var defaultRole = await _dbContext.Roles.FirstOrDefaultAsync(r => r.Name == "User");
                if (defaultRole == null)
                {
                    return new TokenResult { Errors = new[] { "Default role 'User' not found in the system." } };
                }

                user.Roles.Add(defaultRole);
                _dbContext.Users.Add(user);
                await _dbContext.SaveChangesAsync();

                return await SendVerificationEmailAsync(email);
            }
            catch (Exception ex)
            {
                return new TokenResult { Errors = new[] { "An error occurred while processing the registration.", $"Error: {ex.Message}" } };
            }
        }

        // 處理登入邏輯
        public async Task<TokenResult> LoginAsync(string username, string password, bool rememberMe)
        {
            try
            {
                var user = await _dbContext.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                {
                    return new TokenResult { Errors = new[] { "Invalid username or password." } };
                }

                if (!PasswordHasher.VerifyPassword(password, user.PasswordHash))
                {
                    return new TokenResult { Errors = new[] { "Invalid username or password." } };
                }

                if (!user.EmailConfirmed)
                {
                    return new TokenResult { Errors = new[] { "Email not verified. Please check your email and verify your account." } };
                }

                var roles = user.Roles?.Select(r => r.Name).ToList();
                if (roles == null || !roles.Any())
                {
                    return new TokenResult { Errors = new[] { "User does not have any assigned roles." } };
                }

                var token = await _jwtService.GenerateTokenAsync(user.Username, roles);

                return new TokenResult
                {
                    AccessToken = token.AccessToken,
                    TokenType = token.TokenType,
                    ExpireMinutes = token.ExpireMinutes,
                    RefreshToken = token.RefreshToken
                };
            }
            catch (Exception ex)
            {
                return new TokenResult { Errors = new[] { "An error occurred while processing the login.", $"Error: {ex.Message}" } };
            }
        }

        // 發送驗證郵件
        public async Task<TokenResult> SendVerificationEmailAsync(string email)
        {
            try 
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    return new TokenResult { Errors = new[] { "No account found with this email." } };
                }

                var emailToken = _jwtService.GenerateEmailToken(email);
                string baseUrl = _configuration["AppSettings:BaseUrl"];

                string verificationLink = $"{baseUrl}/api/Account/verify?token={emailToken}";

                var mailRequest = new MailRequest
                {
                    ToEmail = email,
                    Subject = "Email Verification",
                    Body = $"Please click the link below to verify your email: {verificationLink}"
                };

                await _emailSender.SendEmailiAsync(mailRequest); // 發送驗證郵件

                return new TokenResult();
            }
            catch (Exception ex)
            {
                return new TokenResult { Errors = new[] { "An unknown error occurred.", $"Error: {ex.Message}" } };
            }
        }
    }
}
