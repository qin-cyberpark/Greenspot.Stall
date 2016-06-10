using Newtonsoft.Json;
using System;

namespace Greenspot.SDK.Vend
{
    [Serializable]
    public class AccessTokenBag
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires")]
        public long Expires { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonProperty("domain_prefix")]
        public string DomainPrefix { get; set; }

        [JsonIgnore]
        public bool HasExpired
        {
            get
            {
                var exprieTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(Expires);
                return exprieTime <= DateTime.UtcNow;
            }
        }
    }
}
