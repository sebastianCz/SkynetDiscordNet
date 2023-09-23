using ContosoUniversity.DAL;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using DSharpPlus.SlashCommands;
using Skynet.Domain.Enum;
using Skynet.Services.CommandHandlingLogic;
using Skynet.Services.Search;

namespace Skynet.Services.LavalinkConnection
{
    public class LavalinkEventsHandlers
    {
        private readonly LavalinkManager _connectionManager;
        private readonly MessageSender _messageSender;
        private SearchEngine _searchEngine;
        private UnitOfWork _unitOfWork;
        public LavalinkEventsHandlers()
        {
            _connectionManager = new LavalinkManager();
            _messageSender = new MessageSender();
            _searchEngine = new SearchEngine();
            _unitOfWork = new UnitOfWork();
        }
        public async Task LogUnknownError(LavalinkGuildConnection sender, EventArgs e)
        {
            Console.WriteLine("An unknown error occured");
        }
        public async Task NodeDisconnected(LavalinkNodeConnection node, NodeDisconnectedEventArgs e)
        {
            Console.WriteLine("Node Disconnected");
        }
        public async Task TrackException(LavalinkGuildConnection sender, TrackExceptionEventArgs e)
        {
            Console.WriteLine("Track Exception");
        }
        public async Task PlaybackStarted(LavalinkGuildConnection sender, TrackStartEventArgs e)
        {
            Console.WriteLine("Playback started");
        }
        public async Task WebSocketClosed(LavalinkNodeConnection sender, SocketErrorEventArgs e)
        {
            Console.WriteLine("Websocket exception ");
        }
        public async Task TrackStuck(LavalinkGuildConnection sender, TrackStuckEventArgs e)
        {
            await _messageSender.SendMessageAsync($"Track is stuck", $" Attempting to reset the song.", sender.Channel, DiscordColor.Green);
            await sender.PauseAsync();
            Thread.Sleep(100);
            await sender.ResumeAsync();
            await _messageSender.SendMessageAsync($"Attempt to unstuck the song finished", $" If you can't hear music type  /stop and /autoPlay or /Play", sender.Channel, DiscordColor.Green);
        }
        public async Task PlaybackEnded(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            try
            {
                //This is running on the same thread as the main app loop, as  a result we basically have a singleton service for autoPlay. 
                //Dsharp plus doesn't have support for DI for events. 
                this._unitOfWork = new UnitOfWork();
                this._searchEngine = new SearchEngine(_unitOfWork);
                var guildInfo = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(sender.Guild.Id.ToString(), x => x.ActivePlaylist);
                if (!guildInfo.AutoplayOn && guildInfo.ActivePlaylist.Count == 0)
                {
                    await _messageSender.SendMessageAsync($"Player stopping", $" Autoplay is turned off and the playlist is empty", sender.Channel, DiscordColor.Green);
                    return;
                }
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Thread ID: {Thread.CurrentThread.ManagedThreadId}||Pending:{ThreadPool.PendingWorkItemCount} || thread count:{ThreadPool.ThreadCount}");
                Console.ForegroundColor = ConsoleColor.White;
                await _connectionManager.OnEventChecks(e.Player.Node.Parent, e.Player.Node, e.Player);
                await _connectionManager.AssureConnected(e.Player, e.Player.Node);
                //Play a random track 
                var searchResult = await _searchEngine.GetSongAsync(sender.Node, sender.Channel);
                if (searchResult.Tracks.Count != 0)
                {
                    var track = searchResult.Tracks.FirstOrDefault();
                    await _messageSender.SendMessageAsync($"Now Playing: {track.Title}", $" Link: {track.Uri}+Lenght:{track.Length}", sender.Channel, DiscordColor.Green);
                    await sender.PlayAsync(track);
                }
                else
                {
                    sender.StopAsync();
                    sender.DisconnectAsync();
                    throw new Exception("No song found during autoplay query. I've disconnected. Type /play and provide a song name to resume ");
                }
            }
            catch (Exception ex)
            {
                var x = new InteractionContext();
                //strack trace should be logged as critical, autoplay will be jammed if we get here
                await _messageSender.SendMessageAsync($"An error occured during autoplay", $"{ex.Message}", sender.Channel, DiscordColor.Green);
                await _messageSender.LogError(x, ex.Message, ex.StackTrace, LoggingLevel.information, sender.Channel);
            }
        }
    }
}
