using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Lavalink.EventArgs;
using Newtonsoft.Json;
using Skynet.db;
using Skynet.Domain;
using Skynet.Domain.Steam;
using Skynet.Services.Interface;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Skynet.Services.LavalinkConnection
{
    public class LavalinkEventsHandlers  
    { 
        private readonly LavalinkConnectionManager _connectionManager;
        private readonly MessageSender _messageSender;
        private readonly SearchEngine _searchEngine;
        public LavalinkEventsHandlers()
        {
            _connectionManager = new LavalinkConnectionManager();
            _messageSender = new MessageSender();
            _searchEngine = new SearchEngine();
        }
        public async Task PlaybackStarted(LavalinkGuildConnection sender, EventArgs e)
        {
            Console.WriteLine("Playback started");
            return;
        }
        public async Task PlaybackEnded(LavalinkGuildConnection sender, TrackFinishEventArgs e)
        {
            try
            { 
                Console.WriteLine($"PlaybackEnded");
                await _connectionManager.OnCommandChecksAsync(e.Player.Node.Parent, e.Player.Node, e.Player);
                await _connectionManager.AssureConnected(e.Player, e.Player.Node);
                var track = await _searchEngine.GetTracksFromGuildPlaylistAsync(e.Player.Node);
                var musicConfig = ReaderJson.DeserializeFile<List<MusicConfig>>("MusicConfig").FirstOrDefault(x => x.Id == sender.Guild.Id.ToString());

                if (track != null)
                {
                    await _messageSender.SendMessage($"Now Playing: {track.Title}", $" Link: {track.Uri}+Lenght:{track.Length}", sender.Channel, DiscordColor.Green);
                    await sender.PlayAsync(track);
                }
                else
                {
                    await _messageSender.SendMessage($"Playlist Ended", $" Autoplay is : {(musicConfig.AutoplayOn ? "on" : "off")}", sender.Channel, DiscordColor.Green);
                }
                if (track == null && musicConfig.AutoplayOn)
                { 
                    track = await _searchEngine.GetRandomTrackAsync(e.Player.Node);
                    if (track != null)
                    {
                        await _messageSender.SendMessage($"Now Playing: {track.Title}", $" Link: {track.Uri}+Lenght:{track.Length}", sender.Channel, DiscordColor.Green);
                        await sender.PlayAsync(track);
                    } 
                }
            }catch(Exception ex)
            {
                //strack trace should be logged as critical, autoplay will be jammed if we get here
                await _messageSender.SendMessage($"An error occured", $"{ex.Message}", sender.Channel, DiscordColor.Green);
            }
        } 
    }
}
