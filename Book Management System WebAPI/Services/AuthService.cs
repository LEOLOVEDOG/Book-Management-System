using Book_Management_System_WebAPI.Interfaces;
using Book_Management_System_WebAPI.Models;
using Book_Management_System_WebAPI.Results;
using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Book_Management_System_WebAPI.Services
{
    public class AuthService: IAuthService
    {
        private readonly BookManagementSystemDbContext _dbContext;
        private readonly GoogleOAuth _googleOAuth;
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;


        public AuthService(BookManagementSystemDbContext dbContext, IOptions<GoogleOAuth> googleOAuth, HttpClient httpClient, IUserService userService)
        {
            _dbContext = dbContext;
            _googleOAuth = googleOAuth.Value;
            _httpClient = httpClient;
            _userService = userService;
        }

        // 取得 Google 登入網址
        public string GoogleLogin()
        {
            var clientId = _googleOAuth.ClientId;
            var redirectUri = _googleOAuth.RedirectUri;

            var url = $"https://accounts.google.com/o/oauth2/auth" +
                      $"?client_id={clientId}" +
                      $"&redirect_uri={redirectUri}" +
                      $"&response_type=code" +
                      $"&scope=openid%20email%20profile";

            return url;
        }

        // 使用 Google 授權碼換取 accesstoken
        public async Task<GoogleTokenResult> ExchangeCodeForTokenAsync(string code)
        {
            var clientId = _googleOAuth.ClientId;
            var clientSecret = _googleOAuth.ClientSecret;
            var redirectUri = _googleOAuth.RedirectUri;

            var tokenRequest = new Dictionary<string, string>
            {
                  { "code", code },
                  { "client_id", clientId },
                  { "client_secret", clientSecret },
                  { "redirect_uri", redirectUri },
                  { "grant_type", "authorization_code" }
            };

            var response = await _httpClient.PostAsync("https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(tokenRequest));

            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error exchanging code for token: {response.StatusCode}, Response: {content}");
            }

            var tokenResult = JsonSerializer.Deserialize<GoogleTokenResult>(content);
            if (tokenResult == null)
            {
                throw new Exception("Failed to deserialize Google token response.");
            }

            return tokenResult;
        }

        // 驗證 Google ID Token
        public async Task<GoogleJsonWebSignature.Payload> VerifyIdTokenAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _googleOAuth.ClientId }
            };

            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}
