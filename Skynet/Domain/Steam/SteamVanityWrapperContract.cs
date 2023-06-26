using System.Text.Json.Serialization;

namespace Skynet.Domain.Steam
{
    public class SteamVanityWrapperContract
    {
        [JsonPropertyName("response")]
        public SteamVanityResultContract Response { get; set; }
        public SteamVanityWrapperContract()
        {

        }
    }
}
