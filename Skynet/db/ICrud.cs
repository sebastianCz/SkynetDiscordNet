using Skynet.Domain;
using Skynet.Domain.Steam;

namespace Skynet.db
{
    public interface ICrud
    {
        public bool PurgeCheaters(string userName, string confirmation);
        public Task<Cheater> AddCheater(SteamPlayer player, string updaterName);
        public Task<Cheater> RemoveCheater(SteamPlayer player, string updaterName);
    }
}
