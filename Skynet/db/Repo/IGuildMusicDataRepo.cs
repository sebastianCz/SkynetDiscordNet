using Microsoft.IdentityModel.Tokens;
using Skynet.Domain.GuildData;
using Skynet.Repository.Repository;
namespace Skynet.db.Repo
{
    public interface IGuildMusiDataRepo<T> : IEntityBaseRepo<GuildMusicData> where T : GuildMusicData
    {
        public  Task<GuildMusicData> GetByDiscordIdAsync(string id);
    }
}
