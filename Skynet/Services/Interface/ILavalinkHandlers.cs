using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;

namespace Skynet.Services.Interface
{
    public interface ILavalinkEventHandlers
    {
        public Task PlaybackStarted(LavalinkGuildConnection sender, EventArgs e);
        public Task PlaybackEnded(LavalinkGuildConnection sender, TrackFinishEventArgs e);
    }
}
