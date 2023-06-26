using Newtonsoft.Json;
using Skynet.Domain.Steam;
using Skynet.Services.Interface;

namespace Skynet.Services
{
    public class SteamApiClient : ISteamApiClient
    {

        private readonly IHttpClientFactory _httpClient;
        public SteamApiClient(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SteamPlayer> GetSteamPlayer(string steamId)
        {
            try
            {
                var response = await _httpClient.CreateClient(SteamApiConfig.SteamApiClientName).GetAsync(
           $"https://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key={SteamApiConfig.AppKey}&steamids={steamId}");
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserialised = JsonConvert.DeserializeObject<SteamResponseWrapper>(responseBody);

                if (deserialised != null)
                {
                    var player = deserialised.Response.Players[0];
                    return player;
                }
                else
                {
                    throw new Exception("Steam provided incorrect data");
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task<string> RetrieveSteamId(string vanityProfile)
        {
            try
            {
                var response = await _httpClient.CreateClient(SteamApiConfig.SteamApiClientName).GetAsync(
                $"http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key={SteamApiConfig.AppKey}&vanityurl={vanityProfile}");
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserialised = JsonConvert.DeserializeObject<SteamVanityWrapperContract>(responseBody);
                if (deserialised.Response.Success == 1)
                {
                    return deserialised.Response.SteamId;
                }
                if (deserialised.Response.Success == 42)
                {
                    throw new Exception("The provided link is correct however user wasn't found on steam");
                }
                throw new Exception("Unkown answer from Steam");
            }
            catch (Exception e)
            {
                throw;
            }


        }
    }
}
