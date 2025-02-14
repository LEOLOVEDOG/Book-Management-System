using Book_Management_System_WebAPI.Results;
using Book_Management_System_WebAPI.Services;
using System.Net.Mail;

namespace Book_Management_System_WebAPI.Interfaces
{
    public interface IUserService
    {
        Task<TokenResult> RegisterAsync(string username, string password, string email);
        Task<TokenResult> LoginAsync(string username, string password, bool remeberme);
        Task<TokenResult> SendVerificationEmailAsync(string email);
    }
}
