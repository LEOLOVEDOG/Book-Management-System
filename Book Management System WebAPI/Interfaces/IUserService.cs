using Book_Management_System_WebAPI.Models;
using Book_Management_System_WebAPI.Requests;
using Book_Management_System_WebAPI.Results;
using Google.Apis.Auth;

namespace Book_Management_System_WebAPI.Interfaces
{
    public interface IUserService
    {
        Task<TokenResult> RegisterAsync(string username, string password, string email);
        Task<TokenResult> LoginAsync(string username, string password, bool remeberme);
        Task<TokenResult> SendVerificationEmailAsync(string email, string emailType);
        Task<TokenResult> ResetPasswordAsync(ResetPasswordRequest request);
        Task<User> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload googleUser);
    }
}
