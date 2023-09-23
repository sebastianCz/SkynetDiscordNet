using DSharpPlus.Lavalink;
using Google.Apis.YouTube.v3.Data;
using Newtonsoft.Json;
using Skynet.Domain.GuildData;
using System.Runtime.CompilerServices;

namespace Skynet.db
{
    public static class Helpers
    {
        public static LavalinkTrackBot ConvertToBotTrack(this LavalinkTrack song)
        {
            var x = JsonConvert.SerializeObject(song);
            var y = JsonConvert.DeserializeObject<LavalinkTrackBot>(x);
            var z = JsonConvert.DeserializeObject <LavalinkTrack>(x);
            
            return y;
        }
        public static LavalinkTrack ConvertToLavalink(this LavalinkTrackBot song)
        {
            //There is no way for me to create a LavalinkTrack object, 
            var x = JsonConvert.SerializeObject(song);
            var y = JsonConvert.DeserializeObject<LavalinkTrack>(x);
            return y;   
        }
    }
}
