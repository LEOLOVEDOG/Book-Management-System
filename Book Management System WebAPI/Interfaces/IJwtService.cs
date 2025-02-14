using Book_Management_System_WebAPI.Results;

namespace Book_Management_System_WebAPI.Interfaces
{
    public interface IJwtService
    {
        Task<TokenResult> GenerateTokenAsync(string username, List<string> roles);
        Task<TokenResult> RefreshTokenAsync(string token, string refreshToken);
        Task<bool> InvalidateTokenAsync(string refreshtoken);
        string GenerateEmailToken(string email);
        Task<TokenResult> VerifyEmail(string token);
    }
}
