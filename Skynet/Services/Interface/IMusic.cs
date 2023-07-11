using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.SlashCommands;

namespace Skynet.Services.Interface
{
    public interface IMusic
    {
        public Task PlayMusic(InteractionContext ctx,string query);
        public Task ResumeMusic(InteractionContext ctx);
        public Task PauseMusic(InteractionContext ctx);
        public Task StopMusic(InteractionContext ctx);
    }
}
