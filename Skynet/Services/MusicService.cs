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
        public MusicService(IMessageSender sender,ILavalinkConnectionManager connectionManager)
        {
                _messageSender = sender;
            _connectionManager = connectionManager;  
        }
        public async Task Shuffle(InteractionContext ctx,string options)
        {
            var isInt = int.TryParse(options.Trim(), out int result);
            if (!isInt || (result != 1 && result != 2))
            {
                throw new Exception("Provided option is incorrect. Make sure you type either 1 or 2");
            }
            if(result == 1)
            {
                var config = await GetConfigShuffle(ctx);
                if (config.ShuffleStatusOn)
                {
                    throw new Exception("Shuffle is already on.If bot was disconnected, it should reconnect now. ");
                    PlayShuffle(ctx,config);
                }
                
                await ShuffleOn(ctx, config); 
            }
            if (result == 2)
            {
                var config = await GetConfigShuffle(ctx);
                if (!config.ShuffleStatusOn)
                {
                    throw new Exception("Shuffle is already off.");
                }
                await ShuffleOff(ctx, await GetConfigShuffle(ctx));
            }

        } 
        private async Task<string> SearchShuffle()
        {
            var random = new Random();
            var searchTermsString = ReaderJson.ReadFile("MusicSearchTerms");
            var searchTerms = JsonConvert.DeserializeObject<List<MusicSearchTerm>>(searchTermsString);
            var searchTermId = random.Next(0, searchTerms.Count);
            return searchTerms[searchTermId].Term;
        }
        private async Task PlayShuffle(InteractionContext ctx,ShuffleConfig config)
        {
            var connection = await _connectionManager.GetConnectionData(ctx);
                List<LavalinkTrack> currentPlaylist;
                ShuffleConfig liveConfig;
                do
                { 
                var searchTerm = await SearchShuffle();
                        var searchResult = await connection.NodeConnection.Rest.GetTracksAsync(searchTerm);
                        if (searchResult.LoadResultType == LavalinkLoadResultType.NoMatches || searchResult.LoadResultType == LavalinkLoadResultType.LoadFailed)
                        { 
                                 _messageSender.SendMessage(ctx, $"Just a couple seconds", $"Failed to find a song with search term {searchTerm}\n Searching again.", DiscordColor.Green);
                    

                        }
                     var musicTrack = searchResult.Tracks.First(); 
                    _messageSender.SendMessage(ctx, $"Now Playing: {musicTrack.Title}", $" Link: {musicTrack.Uri}", DiscordColor.Green);
                    await connection.GuildConnection.PlayAsync(musicTrack);
                    var timer = musicTrack.Length;
                    await Task.Delay(timer); 

                    var liveConfigString = ReaderJson.ReadFile("LiveConfig");
                    liveConfig = JsonConvert.DeserializeObject<ShuffleConfig>(liveConfigString);
                    
                }
                while (liveConfig.ShuffleStatusOn);
                    await connection.GuildConnection.StopAsync();
                    await connection.GuildConnection.DisconnectAsync();
                _messageSender.SendMessage(ctx, $"Shuffle Ended", "Type /play or /shuffle for music to resume.", DiscordColor.Green);
            

        }
        private async Task<ShuffleConfig> GetConfigShuffle(InteractionContext ctx)
        {
            var configString = ReaderJson.ReadFile("LiveConfig");
            var shuffleConfig = JsonConvert.DeserializeObject<ShuffleConfig>(configString);
            return shuffleConfig;
        }
        private async Task ShuffleOff(InteractionContext ctx,ShuffleConfig config)
        {
            config.ShuffleStatusOn = false;
            var configString = JsonConvert.SerializeObject(config);
            ReaderJson.SaveFile("LiveConfig", configString);
            await _messageSender.SendMessage(ctx, "Shuffle Stopped", "Next song won't be selected.", DiscordColor.Green);

            var connection = await _connectionManager.GetConnectionData(ctx);  
            await connection.GuildConnection.DisconnectAsync();
        }
        private async Task ShuffleOn(InteractionContext ctx,ShuffleConfig config)
        {
            config.ShuffleStatusOn = true;
            var configString = JsonConvert.SerializeObject(config);
            ReaderJson.SaveFile("LiveConfig", configString);
            PlayShuffle(ctx,config);
            await _messageSender.SendMessage(ctx,"Shuffle Started", "Song should be playing now", DiscordColor.Green);


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
            await _messageSender.SendMessage(ctx,"Playlist Summary",msg, DiscordColor.Yellow);
        }
   
        public async Task PlayMusic(InteractionContext ctx, string query)
        {
            var connection = await _connectionManager.GetConnectionData(ctx); 
            var searchQuery = await connection.NodeConnection.Rest.GetTracksAsync(query); 
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                throw new Exception("Failed to find music corresponding to your query");
            } 
            var musicTrack = searchQuery.Tracks.First(); 
            
            if (connection.GuildConnection.CurrentState.CurrentTrack != null)
            {
                await _messageSender.SendMessage(ctx, "A track is already playing! ", $" Your song:{musicTrack.Title} was added to playlist ",DiscordColor.Yellow);
                var task1 = AddToPlaylist(musicTrack);
                var task2 = AddToArchive(musicTrack);
                var task3 = AddSearchTermToList(query, ctx);
                var taskList = new List<Task> { task1, task2, task3 };
                await Task.WhenAll(taskList); 
            }
            else
            {  
                _messageSender.SendMessage(ctx, $"Now Playing: {musicTrack.Title}", $" Link: {musicTrack.Uri}+Lenght:{musicTrack.Length}", DiscordColor.Green);
                await connection.GuildConnection.PlayAsync(musicTrack);     
            }
            
        }

        public async Task Skip(InteractionContext ctx)
        {
            var connection = await _connectionManager.GetConnectionData(ctx);
            var currentPlaylist = ReaderJson.DeserializeFile<List<LavalinkTrack>>("MusicPlaylist");
            var firstSong = currentPlaylist.FirstOrDefault();
            if (firstSong == null)
            {
                throw new Exception("Playlist is empty");
            }
            currentPlaylist.Remove(firstSong);
            ReaderJson.SerialiseAndSave(currentPlaylist, "MusicPlaylist");
            await _messageSender.SendMessage(ctx, $"Now Playing: {firstSong.Title}", $" Link: {firstSong.Uri}", DiscordColor.Green);

            var searchQuery = await connection.NodeConnection.Rest.GetTracksAsync(firstSong.Uri);
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                throw new Exception("Failed to find music corresponding to your query. The next song URL was saved with an error.");
            }
            var musicTrack = searchQuery.Tracks.First();
            await connection.GuildConnection.PlayAsync(musicTrack);

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
                _messageSender.SendMessage(ctx, $"Track Paused", $" Type /Resume to keep listening", DiscordColor.Yellow); 
            
        }
        public async Task ResumeMusic(InteractionContext ctx)
        {
            var connection = await _connectionManager.GetConnectionData(ctx);
            await connection.GuildConnection.ResumeAsync();
            _messageSender.SendMessage(ctx, $"Track Resumed", $" Music should be now playing", DiscordColor.Green);
        }
        public async Task StopMusic(InteractionContext ctx)
        {
            if (!await _connectionManager.IsConnectedAsync(ctx)) { throw new Exception("Music isn't playing right now!");} 
            var guildConnection = await _connectionManager.GetGuildConnection(ctx);
            await guildConnection.StopAsync();
            await guildConnection.DisconnectAsync();
            _messageSender.SendMessage(ctx, $"Music stopped", $" Turns out you can stop Rock'N'Roll", DiscordColor.Red);
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
