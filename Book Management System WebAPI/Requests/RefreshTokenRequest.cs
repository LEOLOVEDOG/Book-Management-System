using System.Text.Json.Serialization;

namespace Book_Management_System_WebAPI.Requests
{
    // RefreshToken 请求参数
    public class RefreshTokenRequest
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
