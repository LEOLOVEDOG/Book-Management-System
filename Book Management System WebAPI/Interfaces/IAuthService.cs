using Book_Management_System_WebAPI.Results;
using Google.Apis.Auth;

namespace Book_Management_System_WebAPI.Interfaces
{
    public interface IAuthService
    {
        string GoogleLogin();
        Task<GoogleTokenResult> ExchangeCodeForTokenAsync(string code);
        Task<GoogleJsonWebSignature.Payload> VerifyIdTokenAsync(string idToken);

    }
}
