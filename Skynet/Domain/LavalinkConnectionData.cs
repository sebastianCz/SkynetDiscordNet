using DSharpPlus.Lavalink;

namespace Skynet.Domain
{
    public class LavalinkConnectionData
    { 
        public LavalinkGuildConnection GuildConnection { get; set; }
        public LavalinkNodeConnection NodeConnection { get; set; }
    }
}
