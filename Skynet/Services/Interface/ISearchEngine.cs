using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using Newtonsoft.Json;
using Skynet.db;
using Skynet.Domain;

namespace Skynet.Services.Interface
{
    public interface ISearchEngine
    {
        public  Task<SearchEngineResult> GetSongAsync(LavalinkNodeConnection node, DiscordChannel channel, string query = null);
     }

}
