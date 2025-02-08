using System.Text.Json.Serialization;

namespace Book_Management_System_WebAPI.Requests
{
    public class InvalidateTokenRequest
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
