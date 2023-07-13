using Newtonsoft.Json;
using Skynet.db;
using Skynet.Domain.Cheaters;
using Skynet.Services.Interface;

namespace Skynet.Services
{
    public class CheaterLogic : ICheaterLogic
    {
        private readonly IValidation _validate;
        private readonly ISteamApiClient _client;
        private readonly ICrud _crud;
        public CheaterLogic(IValidation validation, ISteamApiClient client, ICrud crud)
        {
            _validate = validation;
            _client = client;
            _crud = crud;
        }
        public async Task<Search> IsCheater(string steamProfileLink)
        {
            var search = new Search();
            var steamId = await _validate.ValidateSteamProfileLink(steamProfileLink);
            search.Result = steamId;
            var player = await _client.GetSteamPlayer(steamId);
            var cheaterJson = ReaderJson.ReadFile("Cheaters");
            var cheaterWrapper = JsonConvert.DeserializeObject<CheaterListWrapper>(cheaterJson);
            foreach (var x in cheaterWrapper.cheaters)
            {
                if (x.SteamId == player.SteamId)
                {
                    search.Found = true;
                    search.Description += "User was found on cheater list.\n";
                    if (search.Found)
                    {
                        var knownName = x.Names.Where(y => y.Contains(player.personaName)).Any();
                        if (knownName)
                        {
                            search.Description += $"User didn't change his name since last verification.\n Known Names: \n{String.Join("\n", x.Names)}";
                        }
                        else
                        {
                            x.Names.Add(player.personaName);
                            search.Description += $"User changed his name since last verification. \n New name added to database.Known names:\n{String.Join("\n", x.Names)}";
                        }

                        break;
                    }
                }
            }
            if (!search.Found)
            {
                search.Description += "User wasn't found on cheaters list\n";
            }
            return search;
        }
        public async Task<Search> AddCheater(string steamProfileLink, string updaterName)
        {

            var search = new Search();
            var steamId = await _validate.ValidateSteamProfileLink(steamProfileLink);
            var player = await _client.GetSteamPlayer(steamId);
            var cheater = await _crud.AddCheater(player, updaterName);
            if (cheater.SteamId != null)
            {
                search.Found = true;
                search.Title = "Cheater Added";
                search.Description += $"Steam Id: {cheater.SteamId}\n Cheater Names: {String.Join("\n", cheater.Names)} ";
            }
            else
            {
                search.Found = false;
                search.Title = "Adding failed";
                search.Description += "Couldn't add user to list of cheaters. He's already added.";
            }
            return search;
        }
        public async Task<Search> DeleteCheater(string steamProfileLink, string updaterName)
        {
            var search = new Search();
            var steamId = await _validate.ValidateSteamProfileLink(steamProfileLink);
            var player = await _client.GetSteamPlayer(steamId);
            var cheater = await _crud.RemoveCheater(player, updaterName);
            if (cheater.SteamId != null)
            {
                search.Found = true;
                search.Title = "Cheater removed";
                search.Description = $"Steam Id: {cheater.SteamId}\n Cheater Names: {String.Join("\n", cheater.Names)} ";
            }
            else
            {
                search.Found = false;
                search.Title = "Removal failed";
                search.Description = "Couldn't remove user. He's not on the list";
            }
            return search;
        }

        public async Task<Search> DisplayAll()
        {
            var cheaterJson = ReaderJson.ReadFile("Cheaters");
            var cheaterWrapper = JsonConvert.DeserializeObject<CheaterListWrapper>(cheaterJson);
            var search = new Search();
            search.Found = true;
            search.Title = "List of all cheaters";

            if (cheaterWrapper.Lenght > 0)
            {
                foreach (var cheater in cheaterWrapper.cheaters)
                {
                    search.Description += $"\n Cheater # {cheater.Id + 1} Cheater Steam Id: {cheater.SteamId} Known Names:{String.Join("/", cheater.Names)} ";
                }
            }
            else
            {
                search.Description = " List is empty.";
            }
            return search;
        }
    }
}
