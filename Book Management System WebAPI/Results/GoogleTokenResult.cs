using System.Text.Json.Serialization;

namespace Book_Management_System_WebAPI.Results
{
    public class GoogleTokenResult
    {

        public bool Success => Errors == null || !Errors.Any();
        public IEnumerable<string>? Errors { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("id_token")]
        public string IdToken { get; set; }
    }
}
