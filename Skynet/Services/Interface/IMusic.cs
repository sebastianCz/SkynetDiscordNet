 
using DSharpPlus.SlashCommands;

namespace Skynet.Services.Interface
{
    public interface IMusic
    {
        public Task PlayMusic(InteractionContext ctx,string query);
        public Task ResumeMusic(InteractionContext ctx);
        public Task PauseMusic(InteractionContext ctx);
        public Task StopMusic(InteractionContext ctx);
        public Task Skip(InteractionContext ctx);
        public Task Clear(InteractionContext ctx);
        public Task ShowPlaylist(InteractionContext ctx,string options);
        public Task Autoplay(InteractionContext ctx, string options);
        public Task CreateCustomPlaylist(InteractionContext ctx, string playlistList);
        public Task RemoveCustomPlaylist(InteractionContext ctx, string stringId);
        public Task ShowCustomPlaylist(InteractionContext ctx);


    }
}
