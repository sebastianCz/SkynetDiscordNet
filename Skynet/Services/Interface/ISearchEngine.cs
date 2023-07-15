using DSharpPlus.Lavalink;
using Newtonsoft.Json;
using Skynet.db;
using Skynet.Domain;

namespace Skynet.Services.Interface
{
    public interface ISearchEngine
    {
        public Task<LavalinkTrack?> GetTracksFromGuildPlaylistAsync(LavalinkNodeConnection node);

        public Task<LavalinkTrack?> GetRandomTrackAsync(LavalinkNodeConnection node );
     }

}
