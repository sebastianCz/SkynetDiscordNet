using System.Text.Json.Serialization;

namespace Skynet.Domain.Steam
{
    public class SteamPlayerList
    {
        [JsonPropertyName("players")]
        public List<SteamPlayer> Players { get; set; }
        public SteamPlayerList()
        {

        }

    }
}
