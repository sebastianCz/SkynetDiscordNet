using System.Text.Json.Serialization;

namespace Skynet.Domain.Steam
{
    public class SteamResponseWrapper
    {
        [JsonPropertyName("response")]
        public SteamPlayerList Response { get; set; }
    }
}
