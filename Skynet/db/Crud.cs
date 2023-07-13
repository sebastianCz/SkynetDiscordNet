using Newtonsoft.Json;
using Skynet.Domain.Cheaters;
using Skynet.Domain.Steam;

namespace Skynet.db
{
    public class Crud : ICrud
    {

        public bool PurgeCheaters(string userName, string confirmation)
        {
            if (confirmation != "DELETE")
            {
                throw new Exception("Wrong data provided. You must type DELETE in capital letters. \n You can't reverse this action");
            }
            var cheaterListExists = ReaderJson.FileExitsInDirectory("Cheaters");
            try
            {
                if (cheaterListExists)
                {
                    var cheatersList = new CheaterListWrapper()
                    {
                        LastUpdated = DateTime.Now,
                        UpdatedBy = userName,
                        cheaters = new List<Cheater>()
                    };

                    var serialisedList = JsonConvert.SerializeObject(cheatersList);
                    ReaderJson.SaveFile("Cheaters", serialisedList);
                    return true;
                }
            }
            catch (Exception e)
            {
                //log it one day   
            }
            return false;
        }

        public async Task<Cheater> AddCheater(SteamPlayer player, string updaterName)
        {
            var cheaterFile = ReaderJson.ReadFile("Cheaters");
            var cheaterWrapper = JsonConvert.DeserializeObject<CheaterListWrapper>(cheaterFile);
            var cheater = new Cheater();

            if (await CheaterExists(player, cheaterWrapper)) { return cheater; }

            cheaterWrapper.LastUpdated = DateTime.Now;
            cheaterWrapper.UpdatedBy = updaterName;
            cheater.Id = cheaterWrapper.Lenght;
            cheater.SteamId = player.SteamId;
            cheater.Names = new List<string>();
            cheater.Added = DateTime.Now;
            cheater.LastVerified = DateTime.Now;
            var parsed = int.TryParse(player.CommunityVisibilityState, out int result);
            // 3 or more  == public profile on steam
            if (parsed) { cheater.PrivateProfile = result < 3; }
            cheater.Names.Add(player.personaName);
            cheater.Id = cheaterWrapper.Lenght;
            cheaterWrapper.cheaters.Add(cheater);
            var serialised = JsonConvert.SerializeObject(cheaterWrapper);
            ReaderJson.SaveFile("Cheaters", serialised);
            return cheater;
        }


        public async Task<Cheater> RemoveCheater(SteamPlayer player, string updaterName)
        {
            var cheaterFile = ReaderJson.ReadFile("Cheaters");
            var cheaterWrapper = JsonConvert.DeserializeObject<CheaterListWrapper>(cheaterFile);
            var cheaterExists = await CheaterExists(player, cheaterWrapper);
            var cheater = new Cheater();
            if (!await CheaterExists(player, cheaterWrapper)) { return cheater; }
            cheater = cheaterWrapper.cheaters.FirstOrDefault(x => x.SteamId == player.SteamId);
            cheaterWrapper.cheaters.Remove(cheater);
            var serialised = JsonConvert.SerializeObject(cheaterWrapper);
            ReaderJson.SaveFile("Cheaters", serialised);
            return cheater;
        }
        private async Task<bool> CheaterExists(SteamPlayer player, CheaterListWrapper list)
        {
            var test = list.cheaters.Where(x => x.SteamId == player.SteamId).Any();
            return test;
        }
    }
}
