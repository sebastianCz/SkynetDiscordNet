using Newtonsoft.Json;

namespace Skynet.Domain
{
    internal struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("prefix")]
        public string Prefix { get; set; }
        [JsonProperty("googleApiKey")]
        public string GoogleApiKey { get; set; }
        [JsonProperty("botPassword")]
        public string BotPassword { get; set; }
    }
}
