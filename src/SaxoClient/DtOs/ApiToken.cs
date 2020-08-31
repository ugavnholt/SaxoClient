using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Saxo.DtOs
{
    public class ApiToken
    {
        public DateTime IssueTime
        {
            get { return DateTime.UtcNow; }
        }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("refresh_token_expires_in")]
        public int RefreshTokenExpiresIn { get; set; }
    }
}
