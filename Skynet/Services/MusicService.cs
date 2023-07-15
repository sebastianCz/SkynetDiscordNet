using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Skynet.db;
using Skynet.Domain;
using Skynet.Services.Interface;
using System.Xml.Linq;

namespace Skynet.Services
{
    public class MusicService : IMusic
    {
        private readonly IMessageSender _messageSender; 
        private readonly ILavalinkConnectionManager _connectionManager;
        private readonly ISearchEngine _searchEngine;
        public MusicService(IMessageSender sender,ILavalinkConnectionManager connectionManager,ISearchEngine searchEngine)
        {
                _messageSender = sender;
            _connectionManager = connectionManager;  
            _searchEngine = searchEngine;   
        }
        public async Task Autoplay(InteractionContext ctx,string options)
        {
            var isInt = int.TryParse(options.Trim(), out int result);
            if (!isInt || (result != 1 && result != 2))
            {
                throw new Exception("Provided option is incorrect. Make sure you type either 1 or 2");
            }
            if(result == 1)
            {
                await _connectionManager.AssureConnected(ctx);
                var config = ReaderJson.DeserializeFile<List<MusicConfig>>("MusicConfig").FirstOrDefault(x => x.Id == ctx.Guild.Id.ToString());
                if (config.AutoplayOn)
                {
                    throw new Exception("Shuffle is already on.");  
                }
                
                await AutoplayOn(ctx, config); 
            }
            if (result == 2)
            {
                var config = ReaderJson.DeserializeFile<List<MusicConfig>>("MusicConfig").FirstOrDefault(x => x.Id == ctx.Guild.Id.ToString());
                if (!config.AutoplayOn)
                {
                    throw new Exception("Shuffle is already off.");
                }
                await AutoplayOff(ctx, ReaderJson.DeserializeFile<List<MusicConfig>>("MusicConfig").FirstOrDefault(x => x.Id == ctx.Guild.Id.ToString()));
            }

        }  
        private async Task AutoplayOff(InteractionContext ctx,MusicConfig config)
        {
            config.AutoplayOn = false; 
            ReaderJson.SerialiseAndSave(config, "MusicConfig");
            await _messageSender.SendMessageAsync(ctx, "Shuffle Stopped", "Next song won't be selected.", DiscordColor.Green); 
            var connection = await _connectionManager.GetConnectionData(ctx);
            await connection.GuildConnection.StopAsync();
            await connection.GuildConnection.DisconnectAsync();
        }
        private async Task AutoplayOn(InteractionContext ctx,MusicConfig config)
        {
            config.AutoplayOn = true;
            ReaderJson.SerialiseAndSave(config, "MusicConfig"); 
            await _messageSender.SendMessageAsync(ctx,"Shuffle Started", "Song should start playing soon.", DiscordColor.Green);
            await PlayMusic(ctx);
        }
        public async Task ShowPlaylist(InteractionContext ctx,string options)
        {
            var msg = "";
            var isInt = int.TryParse(options.Trim(), out int result);
            if (!isInt || (result != 1 && result != 2))
            {
                throw new Exception("Provided option is incorrect. Make sure you type either 1 or 2");
            }  
                var musicString = ReaderJson.ReadFile("MusicPlaylist");
                var playlist = JsonConvert.DeserializeObject<List<LavalinkTrack>>(musicString);
                  msg += $"Total tracks on playlist: {playlist.Count} '\n";
                if (playlist.Count != 0 && result == 2)
                {
                    for (var i = 0; i < playlist.Count; i++)
                    {
                        msg += $"#{0 + 1} // {playlist[i].Title}\n";
                    }
              } 
            await _messageSender.SendMessageAsync(ctx,"Playlist Summary",msg, DiscordColor.Yellow);
        }

       
    public async Task PlayMusic(InteractionContext ctx, string query="NA")
        {
            var connection = await _connectionManager.GetConnectionData(ctx);
            LavalinkTrack searchQuery;
            if(query == "NA")
            {
                searchQuery = await _searchEngine.GetRandomTrackAsync(connection.NodeConnection);
                
            }
            else
            {
                var loadResult = await connection.NodeConnection.Rest.GetTracksAsync(query);
                searchQuery = loadResult.Tracks.FirstOrDefault();
            } 

            if (connection.GuildConnection.CurrentState.CurrentTrack != null)
            {
                await _messageSender.SendMessageAsync(ctx, "A track is already playing! ", $" Your song:{searchQuery.Title} was added to playlist ",DiscordColor.Yellow);
                var task1 = AddToPlaylist(searchQuery);
                var task2 = AddToArchive(searchQuery);
                var task3 = AddSearchTermToList(query, ctx);
                var taskList = new List<Task> { task1, task2, task3 };
                await Task.WhenAll(taskList); 
            }
            else
            {  
                await _messageSender.SendMessageAsync(ctx, $"Now Playing: {searchQuery.Title}", $" Link: {searchQuery.Uri}+Lenght:{searchQuery.Length}", DiscordColor.Green); 
                await connection.GuildConnection.PlayAsync(searchQuery);     
            } 
        }

        public async Task Skip(InteractionContext ctx)
        {
            var connection = await _connectionManager.GetConnectionData(ctx);
            await connection.GuildConnection.StopAsync(); 
        }

        public async Task Clear(InteractionContext ctx)
        {
            var newList = new List<LavalinkTrack>();
            ReaderJson.SerialiseAndSave(newList, "MusicPlaylist");
        }
        public async Task AddToArchive(LavalinkTrack track)
        {
            var currentPlaylist = ReaderJson.DeserializeFile<List<LavalinkTrack>>("PlaylistArchive");
            currentPlaylist.Add(track); 
            ReaderJson.SerialiseAndSave(currentPlaylist, "PlaylistArchive");
        }
        public async Task AddToPlaylist(LavalinkTrack track)
        { 
            var currentPlaylist = ReaderJson.DeserializeFile<List<LavalinkTrack>>("MusicPlaylist"); 
            currentPlaylist.Add(track); 
            ReaderJson.SerialiseAndSave(currentPlaylist, "MusicPlaylist");
        }
        public async Task PauseMusic(InteractionContext ctx)
        {
            var connection = await _connectionManager.GetConnectionData(ctx);
              await connection.GuildConnection.PauseAsync();
                _messageSender.SendMessageAsync(ctx, $"Track Paused", $" Type /Resume to keep listening", DiscordColor.Yellow); 
            
        }
        public async Task ResumeMusic(InteractionContext ctx)
        {
            var connection = await _connectionManager.GetConnectionData(ctx);
            await connection.GuildConnection.ResumeAsync();
            _messageSender.SendMessageAsync(ctx, $"Track Resumed", $" Music should be now playing", DiscordColor.Green);
        }
        public async Task StopMusic(InteractionContext ctx)
        {
            if (!await _connectionManager.IsConnectedAsync(ctx)) { throw new Exception("Music isn't playing right now!");} 
            var guildConnection = await _connectionManager.GetGuildConnection(ctx);
            await guildConnection.StopAsync();
            await guildConnection.DisconnectAsync();
            _messageSender.SendMessageAsync(ctx, $"Music stopped", $" Turns out you can stop Rock'N'Roll", DiscordColor.Red);
        }
          
        private async Task AddSearchTermToList(string term, InteractionContext ctx)
        {
            var allUsers = ReaderJson.DeserializeFile<List<MusicUser>>("MusicUsers");
            var searchTerms = ReaderJson.DeserializeFile<List<MusicSearchTerm>>("MusicSearchTerms"); 

            var searTerm = searchTerms.FirstOrDefault(x => x.Term == term);
            if (searTerm == null)
            {
                searTerm = new MusicSearchTerm { AddedOn = DateTime.Now.Date, Term = term };
                searchTerms.Add(searTerm);
                UpdateMusicUser(searTerm, allUsers, ctx);
            }
            else
            {
                UpdateMusicUser(searTerm, allUsers, ctx);
            } 
            ReaderJson.SerialiseAndSave(allUsers, "MusicUsers");
            ReaderJson.SerialiseAndSave(allUsers, "MusicSearchTerms"); 
        }
        private async Task UpdateMusicUser(MusicSearchTerm term, List<MusicUser> allUsers, InteractionContext ctx)
        {
            var user = allUsers.FirstOrDefault(x => x.UserName == ctx.Member.Username);
            if (user != null)
            {
                if (user.SearchTerms.FirstOrDefault(x => x.Term == term.Term) == null)
                {
                    user.SearchTerms.Add(term);
                }
                user.LastUsed = DateTime.Now.Date;
            }
            else
            {
                user = new MusicUser
                {
                    UserName = ctx.Member.Username,
                    LastUsed = DateTime.Now.Date,
                    SearchTerms = new List<MusicSearchTerm>()
                };
                user.SearchTerms.Add(term);
                allUsers.Add(user);
            }
        }

    }
}
