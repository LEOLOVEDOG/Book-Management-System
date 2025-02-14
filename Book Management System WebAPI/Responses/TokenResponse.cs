using System.Text.Json.Serialization;

namespace Book_Management_System_WebAPI.Responses
{
    // 登入 成功后返回 token
    public class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }  // add

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } // add
    }
}
