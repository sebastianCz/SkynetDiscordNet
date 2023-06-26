using Skynet.Services.Interface;

namespace Skynet.Services
{
    public class ValidateInput : IValidation
    {
        private readonly ISteamApiClient _client;
        public ValidateInput(ISteamApiClient client)
        {
            _client = client;
        }
        public async Task<string> ValidateSteamProfileLink(string steamProfileLink)
        {
            var steamLink = steamProfileLink.Split("/");
            var steamId = "";
            if (steamLink.Length > 4 && steamLink[3].Contains("id"))
            {
                steamId = await _client.RetrieveSteamId(steamLink[4]);
                if (steamId.Length > 0) { return steamId; }
                else { throw new Exception("An error occured during steam link validation"); }
            }
            else if (steamLink.Length > 4 && steamLink[3].Contains("profiles"))
            {
                return steamLink[4];
            }
            else
            {
                throw new Exception("A link was provided but it cannot be processed." +
                    " Make sure your link is valid and contains either 'id' or 'profiles' within it." +
                    "If neither is present in the link, it cannot be analysed. ");
            }
        }
    }
}
