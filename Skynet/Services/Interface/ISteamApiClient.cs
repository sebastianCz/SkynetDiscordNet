using Skynet.Domain.Steam;

namespace Skynet.Services.Interface
{
    public interface ISteamApiClient
    {
        public Task<string> RetrieveSteamId(string vanityProfile);
        public Task<SteamPlayer> GetSteamPlayer(string steamId);

    }
}
