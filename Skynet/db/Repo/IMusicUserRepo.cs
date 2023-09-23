using Microsoft.IdentityModel.Tokens;
using Skynet.Domain.GuildData;
using Skynet.Repository.Repository;
namespace Skynet.db.Repo
{
    public interface IMusicUserRepo<T> : IEntityBaseRepo<MusicUser> where T : MusicUser
    {
        public  Task<MusicUser> GetByDiscordIdAsync(string id);
        public  Task<MusicUser> GetUserForTermAddition(string name);
        public  Task<MusicUser> EnsureCreated(string userName);
    }
}
