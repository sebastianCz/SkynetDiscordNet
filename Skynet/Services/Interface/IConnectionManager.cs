using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Domain;

namespace Skynet.Services.Interface
{
    public interface ILavalinkConnectionManager
    {
        public Task OnCommandChecksAsync(InteractionContext ctx);
        public Task OnCommandChecksAsync(LavalinkExtension link,LavalinkNodeConnection node, LavalinkGuildConnection guild);
        public bool IsConnectedToVoice(InteractionContext ctx);
        public Task AssureConnected(InteractionContext ctx);
        public Task AssureConnected(LavalinkGuildConnection guild, LavalinkNodeConnection node); 
        public void ValidateVC(InteractionContext ctx);
    }
}
