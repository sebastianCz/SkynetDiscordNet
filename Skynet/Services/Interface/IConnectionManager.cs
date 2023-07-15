using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Domain;

namespace Skynet.Services.Interface
{
    public interface ILavalinkConnectionManager
    {
        public Task OnCommandChecksAsync(InteractionContext ctx);
        public Task OnCommandChecksAsync(LavalinkExtension link,LavalinkNodeConnection node, LavalinkGuildConnection guild);
        public Task<bool> IsConnectedAsync(InteractionContext ctx);
        public Task AssureConnected(InteractionContext ctx);
        public Task AssureConnected(LavalinkGuildConnection guild, LavalinkNodeConnection node);
        public Task<LavalinkGuildConnection> GetGuildConnection(InteractionContext ctx);
        public Task<LavalinkNodeConnection> GetNodeConnection(InteractionContext ctx);
        public Task<LavalinkConnectionData> GetConnectionData(InteractionContext ctx);
    }
}
