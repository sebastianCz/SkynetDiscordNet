using ContosoUniversity.DAL;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.db;
using Skynet.Domain.GuildData;
using Skynet.Services;
using Skynet.Services.Interface;
using Skynet.Services.LavalinkConnection;
using Skynet.Services.Search;

namespace Skynet.Services.CommandHandlingLogic
{
    public class MusicService : IMusic
    {
        private readonly MessageSender _messageSender;
        private readonly LavalinkManager _connectionManager;
        private readonly SearchEngine _searchEngine;
        private readonly UnitOfWork _unitOfWork;
        public MusicService()
        {
            _messageSender = new MessageSender();
            _connectionManager = new LavalinkManager();
            _searchEngine = new SearchEngine();
            _unitOfWork = new UnitOfWork();
        }
        public async Task SendMusicReport(InteractionContext ctx)
        {
            var guildInfo = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Member.Guild.Id.ToString());
            var report = await _unitOfWork.GuildMusicData.GenerateGuildReportAsync(guildInfo.Id);

            await _messageSender.SendMessageAsync("Music Report Complete",
                $"\n====================" +
                $"\nIf lavalink search type is equal to plain, it means usually a direct URL is used instead of a search query." +
                $"\n====================" +
                $"\nTotal Searches                          :{report.SearchesCount}" +
                $"\nMost popular source                     :{report.MostCommonLavalinkSearchType}" +
                $"\nMost popular search pattern:            :{report.MostCommonSearchPattern}" +
                $"\nUsually searches are initiated through  :{report.MostCommonSearchInput}" +
                $"\nMost popular song                       :{report.MostCommonSongName}" +
                $"\nMost popular URL                        :{report.MostCommonSongUri}",
                ctx.Channel,
                DiscordColor.Green);
        }

        public async Task CreateCustomPlaylist(InteractionContext ctx, string playlistList)
        {
            var playlist = await _searchEngine.GetSongAsync(ctx.GetNode(), ctx.Channel, playlistList);
            if (!playlist.PlaylistReceived)
            {
                throw new Exception("Your playlist wasn't found. Make sure it's not set to private on youtube and that your link is correct.");
            }
            var guildData = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Guild.Id.ToString(), x => x.Playlists);
            if (guildData.Playlists.Where(x => x.Name == playlist.PlaylistName).ToList().Any())
            {
                throw new Exception("Playlist is already added(Playlist with the same name exists)");
            }

            MusicPlaylist newPlaylist = new MusicPlaylist();
            newPlaylist.Name = playlist.PlaylistName;
            newPlaylist.CreatedBy = ctx.Member.Username;
            newPlaylist.DateTime = DateTime.Now;

            foreach (var z in playlist.Tracks) { newPlaylist.Playlist.Add(z.ConvertToBotTrack()); }
            guildData.Playlists.Add(newPlaylist);
            await _unitOfWork.SaveAsync();
            await _messageSender.SendMessageAsync(ctx, "Added playlist ", $"Playlist name: {playlist.PlaylistName} Song count:{playlist.Tracks.Count}", DiscordColor.Green);
            _unitOfWork.Dispose();
        }
        public async Task RemoveCustomPlaylist(InteractionContext ctx, string stringId)
        {
            var isInt = int.TryParse(stringId.Trim(), out int result);
            if (!isInt)
            {
                throw new Exception("Provided Id is incorrect. Make sure you provide a number.");
            }
            //We correct id. Playlists don't have id but a position in the list instead. 
            result = result - 1;
            var guildConfig = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Member.Guild.Id.ToString(), x => x.Playlists);
            if (result > guildConfig.Playlists.Count - 1 || result < 0)
            {
                throw new Exception("Provided number is too high or too low.");
            }
            for (var i = 0; i < guildConfig.Playlists.Count; i++)
            {
                if (i == result)
                {
                    await _messageSender.SendMessageAsync(ctx, "Playlist was removed", guildConfig.Playlists[i].Name, DiscordColor.Green);
                    await _unitOfWork.GuildMusicData.DeletePlaylist(guildConfig.Playlists[i].Id);
                    await _unitOfWork.SaveAsync();
                    _unitOfWork.Dispose();
                    continue;
                }
            }

        }
        public async Task ShowCustomPlaylist(InteractionContext ctx)
        {
            var guildConfig = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Member.Guild.Id.ToString(), x => x.Playlists);
            var msg = $"Total playlists count: {guildConfig.Playlists.Count}\n";
            for (var i = 0; i < guildConfig.Playlists.Count; i++)
            {
                msg += $"Id: {i + 1} # Playlist Name: {guildConfig.Playlists[i].Name}\n";
            }
            _unitOfWork.Dispose();
            await _messageSender.SendMessageAsync(ctx, "Playlist Data", msg, DiscordColor.Green);
        }
        public async Task Autoplay(InteractionContext ctx, string options)
        {
            var guildData = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Member.Guild.Id.ToString());
            var isInt = int.TryParse(options.Trim(), out int result);
            if (!isInt || result != 1 && result != 2)
            {
                throw new Exception("Provided option is incorrect. Make sure you type either 1 or 2");
            }
            if (result == 1)
            {
                if (guildData.AutoplayOn)
                {
                    await _messageSender.SendMessageAsync(ctx, "Autoplay is already on", "If an issue occured, type /stop and /next", DiscordColor.Green);
                }
                await _messageSender.SendMessageAsync(ctx, "Autoplay is now on", "Type /next if no song is playing currently.", DiscordColor.Green);
                guildData.AutoplayOn = true;
                await _unitOfWork.SaveAsync();
                _unitOfWork.Dispose();
            }
            if (result == 2)
            {
                if (!guildData.AutoplayOn) { throw new Exception("Shuffle is already off."); }
                guildData.AutoplayOn = false;

                await _unitOfWork.SaveAsync();
                _unitOfWork.Dispose();
                await _messageSender.SendMessageAsync(ctx, "Autoplay Stopped", "Song won't play automatically from now on.", DiscordColor.Green);
            }

        }
        public async Task ShowPlaylist(InteractionContext ctx, string options)
        {
            var msg = "";
            var isInt = int.TryParse(options.Trim(), out int result);
            if (!isInt || result != 1 && result != 2)
            {
                throw new Exception("Provided option is incorrect. Make sure you type either 1 or 2");
            }

            var guildConfig = await _unitOfWork.GuildMusicData.GetActivePlaylistAsync(ctx.Member.Guild.Id.ToString());
            var playlist = guildConfig.ActivePlaylist;

            msg += $"Total tracks on playlist: {playlist.Count} '\n";
            if (playlist.Count != 0 && result == 2)
            {
                var tempMsg = "";
                for (var i = 0; i < playlist.Count; i++)
                {
                    tempMsg += $"#{i + 1} | {playlist[i].Title}  \n";
                }
                if (tempMsg.Count() >= 4000)
                {
                    msg += "Playlist is too long to display the details";
                    await _messageSender.SendMessageAsync(ctx, "Playlist Summary", msg, DiscordColor.Yellow);
                    return;
                }
                else
                {
                    msg += tempMsg;
                }
            }
            await _messageSender.SendMessageAsync(ctx, "Playlist Summary", msg, DiscordColor.Yellow);
            _unitOfWork.Dispose();
        }


        public async Task PlayMusic(InteractionContext ctx, string query = null)
        {
            LavalinkTrack musicToPlay;
            var guildInfo = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Member.Guild.Id.ToString(), x => x.SearchTerms, x => x.Playlists);
            var searchResult = await _searchEngine.GetSongAsync(ctx.GetNode(), ctx.Channel, query);

            if (searchResult.Tracks.Count == 0)
            {
                return;
            }
            musicToPlay = searchResult.Tracks.FirstOrDefault();
            if (searchResult.PlaylistReceived)
            {
                searchResult.Tracks.Remove(searchResult.Tracks.FirstOrDefault());
                guildInfo.ActivePlaylist.AddRange(searchResult.Tracks.ConvertToBotTrack());
            }

            if (ctx.GetGuildConnection().CurrentState.CurrentTrack != null)
            {
                var msg = "";
                if (searchResult.PlaylistReceived) { msg += " Playlist received and added. Next song: \n"; }
                await _messageSender.SendMessageAsync(ctx, "A track is already playing! ", $"{msg}{musicToPlay.Title} was added to playlist ", DiscordColor.Yellow);
                guildInfo.ActivePlaylist.Add(musicToPlay.ConvertToBotTrack());
                await AddSearchTermToList(query, ctx, guildInfo);
            }
            else
            {
                var msg = ""; if (searchResult.PlaylistReceived) { msg += " Playlist received and added. Next song: \n"; }
                //await  _unitOfWork.GuildMusicData.AddSearchResult(guildInfo,searchResult); 
                await _messageSender.SendMessageAsync(ctx, $"{msg}Now Playing: {musicToPlay.Title}", $" Link: {musicToPlay.Uri}+Lenght:{musicToPlay.Length}", DiscordColor.Green);
                await ctx.GetGuildConnection().PlayAsync(musicToPlay);
                Console.WriteLine($"Thread name:{Thread.CurrentThread.Name} State: {Thread.CurrentThread.ThreadState}  //Play method complete");
            }
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
        }
        private async Task AddSearchTermToList(string term, InteractionContext ctx, GuildMusicData guildData)
        {
            var user = await _unitOfWork.MusicUser.EnsureCreated(ctx.Member.Username);
            var searchTerm = user.SearchTerms.FirstOrDefault(x => x.Term == term);
            if (searchTerm == null)
            {
                searchTerm = new MusicSearchTerm { AddedOn = DateTime.Now.Date, Term = term, GuildMusicData = guildData };
                UpdateMusicUser(user, searchTerm, ctx);
                user.SearchTerms.Add(searchTerm);
            }
            else
            {
                UpdateMusicUser(user, searchTerm, ctx);
            }
        }
        private void UpdateMusicUser(MusicUser user, MusicSearchTerm term, InteractionContext ctx)
        {
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
            }
        }
        public async Task Skip(InteractionContext ctx)
        {
            Console.WriteLine($"Thread name:{Thread.CurrentThread.Name} State: {Thread.CurrentThread.ThreadState}//Entered skip method ");
            var guildInfo = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Member.Guild.Id.ToString(), x => x.ActivePlaylist);
            if (!guildInfo.AutoplayOn && guildInfo.ActivePlaylist.Count == 0)
            {
                await _messageSender.SendMessageAsync(ctx, "Can't proceed to next song", "Autoplay is off and the current playlist has no songs to play.\nType /play or enable autoplay.", DiscordColor.Yellow);
                await _connectionManager.DisconnectAsync(ctx);
                return;
            }
            await _connectionManager.AssureConnected(ctx);
            if (ctx.GetNode().IsConnected && ctx.GetGuildConnection().IsConnected && ctx.GetGuildConnection().CurrentState.CurrentTrack != null)
            {
                if (guildInfo.AutoplayOn) { await _messageSender.SendMessageAsync(ctx, "Song skipped", "Autoplay is on. Next music will play soon.", DiscordColor.Yellow); }
                if (!guildInfo.AutoplayOn) { await _messageSender.SendMessageAsync(ctx, "Song skipped", "Autoplay is off. ", DiscordColor.Yellow); }
                await ctx.GetGuildConnection().StopAsync();
                Console.WriteLine($"Thread name:{Thread.CurrentThread.Name} State: {Thread.CurrentThread.ThreadState}//Left skip method Hour{DateTime.Now}");
            }
            else if (ctx.GetNode().IsConnected && ctx.GetGuildConnection().IsConnected && ctx.GetGuildConnection().CurrentState.CurrentTrack == null)
            {
                await _messageSender.SendMessageAsync(ctx, "No musing currently playing", "Passing to next song", DiscordColor.Yellow);
                await PlayMusic(ctx);
                Console.WriteLine($"Thread name:{Thread.CurrentThread.Name} State: {Thread.CurrentThread.ThreadState}//Left skip method.Hour:{DateTime.Now}");
            }
            else { throw new Exception("Autoplay encountered a critical issue when switching tracks. Reconnect the bot."); }
        }

        public async Task Clear(InteractionContext ctx)
        {
            await _unitOfWork.GuildMusicData.ClearActivePlaylist(ctx.Member.Guild.Id.ToString());
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
        }

        public async Task PauseMusic(InteractionContext ctx)
        {
            await ctx.GetGuildConnection().PauseAsync();
            await _messageSender.SendMessageAsync(ctx, $"Track Paused", $" Type /Resume to keep listening", DiscordColor.Yellow);

        }
        public async Task ResumeMusic(InteractionContext ctx)
        {
            await ctx.GetGuildConnection().ResumeAsync();
            await _messageSender.SendMessageAsync(ctx, $"Track Resumed", $" Music should be now playing", DiscordColor.Green);
        }
        public async Task StopMusic(InteractionContext ctx)
        {
            if (!_connectionManager.IsConnectedToVoice(ctx)) { throw new Exception("Music isn't playing right now!"); }
            var guildInfo = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(ctx.Member.Guild.Id.ToString());
            guildInfo.AutoplayOn = false;
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
            var guildConnection = ctx.GetGuildConnection();
            if (guildConnection.CurrentState.CurrentTrack != null)
            {
                await guildConnection.StopAsync();
            }
            await guildConnection.DisconnectAsync();

            _messageSender.SendMessageAsync(ctx, $"Music stopped", $" Turns out you can stop Rock'N'Roll", DiscordColor.Red);
        }
    }
}
