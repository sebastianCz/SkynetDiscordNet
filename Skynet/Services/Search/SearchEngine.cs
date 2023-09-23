using ContosoUniversity.DAL;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using Skynet.db;
using Skynet.Domain;
using Skynet.Domain.Enum;
using Skynet.Domain.GuildData;
using Skynet.Services.CommandHandlingLogic;
using Skynet.Services.Interface;

namespace Skynet.Services.Search
{

    public class SearchEngine : ISearchEngine
    {
        private readonly MessageSender _messageSender;
        private readonly UnitOfWork _unitOfWork;

        public SearchEngine(UnitOfWork work)
        {
            _messageSender = new MessageSender();
            _unitOfWork = work;
        }
        public SearchEngine()
        {
            _messageSender = new MessageSender();
            _unitOfWork = new UnitOfWork();
        }
        public async Task<SearchEngineResult> GetSongAsync(LavalinkNodeConnection node, DiscordChannel channel, string query = null)
        {
            IEnumerable<LavalinkTrack> loadResult;
            var result = new SearchEngineResult();

            result.PlaylistReceived = false;
            var guildData = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(channel.GuildId.ToString(), x => x.ActivePlaylist, x => x.SearchTerms, x => x.SearchProbability, x => x.Playlists);
            result.GuildMusicDataId = guildData.Id;
            if (!guildData.AutoplayOn && guildData.ActivePlaylist.Count == 0 && query == null)
            {
                await _messageSender.SendMessageAsync($"Player stopping", $" Autoplay is turned off and the playlist is empty", channel, DiscordColor.Yellow);
                return result;
            }
            try
            {
                switch (query)
                {
                    case null:
                        result.SearchInput = SearchInput.AutoPlay;
                        result = await AutoPlayAsync(result, node, channel, guildData);

                        break;
                    default:
                        result.SearchInput = SearchInput.Query;
                        result.SearchType = SearchType.ManualGetTrackLavalink;
                        if (query.Count() == 0)
                        {
                            throw new Exception("Provided query was empty and wouldn't get a match.");
                        }
                        else if (query.Contains("soundcloud.com"))//This will fire if the query is a link to those platforms.
                        {
                            result.LavalinkSearchType = LavalinkSearchTypeInt.Plain;
                            result = await GetTracksFromLavalinkAsync(result, node, query, LavalinkSearchType.Plain);

                        }
                        else if (query.Contains("youtube.com"))//This will fire if the query is a link to those platforms.
                        {
                            result.LavalinkSearchType = LavalinkSearchTypeInt.Plain;
                            result = await GetTracksFromLavalinkAsync(result, node, query, LavalinkSearchType.Plain);
                        }
                        else
                        {
                            query += " music";
                            result = await GetTracksFromLavalinkAsync(result, node, query, LavalinkSearchType.SoundCloud);
                            if (result.Tracks.Count == 0) { result = await GetTracksFromLavalinkAsync(result, node, query, LavalinkSearchType.Youtube); }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                await _messageSender.SendMessageAsync($"Error occured while looking for song.", ex.Message, channel, DiscordColor.Yellow);
                throw;
            }
            if (result.Tracks.Count() <= 0)
            {
                result.FinaliseSearchResult(false);
                throw new Exception($"No tracks were found.Search Query: {query}");
            }
            result.FinaliseSearchResult(true);
            guildData.SearchEngines.Add(result);
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
            return result;
        }

        private async Task<SearchEngineResult> AutoPlayAsync(SearchEngineResult result, LavalinkNodeConnection node, DiscordChannel channel, GuildMusicData guildData)
        {
            LavalinkTrack track;
            var weightedList = new List<(Func<KeyValuePair<LavalinkNodeConnection, string>, Task<(LavalinkTrack, LavalinkSearchTypeInt)>>, int)>();
            var methodsParameters = new KeyValuePair<LavalinkNodeConnection, string>(node, channel.GuildId.ToString());
            //Active playlist from /play command always gets the priority
            var tuple = await GetTracksFromActivePlaylist(methodsParameters);
            track = tuple.Item1;
            if (track != null) { result.SearchType = SearchType.AutoplayActivePlaylist; result.LavalinkSearchType = tuple.Item2; result.Tracks.Add(track); return result; }
            if (!guildData.AutoplayOn) { return result; }

            if (guildData.Playlists.Count > 0)
            {
                weightedList.Add((GetRandomTrackFromGuildPlaylist, guildData.SearchProbability.AutoplayGuildPlaylists));
            }
            if (guildData.SearchTerms.Count > 30)
            {
                weightedList.Add((GetRandomTracksFromGuildSearchTerm, guildData.SearchProbability.AutoPlayUserTerms));
            }
            weightedList.Add((GetDefaultTracks, guildData.SearchProbability.AutoPlayDefaultTracks));

            if (weightedList.Count <= 0)
            {
                throw new Exception("No data source for random tracks.Contact developpers.This really isn't supposed to happen.");
            }
            var randomFunction = weightedList.GetRandomWeightedElement();
            foreach (var attribute in randomFunction.Method.CustomAttributes.First(x => x.AttributeType == typeof(SearchMethodAttribute)).ConstructorArguments)
            {
                if (attribute.ArgumentType == typeof(SearchType)) { result.SearchType = (SearchType)attribute.Value; }
            }
            var search = await randomFunction(methodsParameters);
            track = search.Item1;
            result.LavalinkSearchType = search.Item2;
            if (track != null) { result.Tracks.Add(track); }
            return result;
        }

        private async Task<SearchEngineResult> GetTracksFromLavalinkAsync(SearchEngineResult result, LavalinkNodeConnection node, string searchQuery, LavalinkSearchType searchType)
        {
            var loadResult = await node.Rest.GetTracksAsync(searchQuery, searchType);
            if (searchType == LavalinkSearchType.Youtube) { result.LavalinkSearchType = LavalinkSearchTypeInt.Youtube; }
            if (searchType == LavalinkSearchType.SoundCloud) { result.LavalinkSearchType = LavalinkSearchTypeInt.SoundCloud; }
            if (loadResult.LoadResultType == LavalinkLoadResultType.NoMatches || loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                return result;
            }
            if (loadResult.Tracks != null && !loadResult.Tracks.Any())
            {
                return result;
            }
            if (loadResult.LoadResultType == LavalinkLoadResultType.PlaylistLoaded)
            {
                result.PlaylistReceived = true;
                result.PlaylistName = loadResult.PlaylistInfo.Name;
            }
            result.Tracks.AddRange(loadResult.Tracks);
            return result;
        }

        private async Task<LavalinkTrack> GetTracksFromLavalinkAsync(LavalinkNodeConnection node, string searchQuery, LavalinkSearchType searchType)
        {
            var loadResult = await node.Rest.GetTracksAsync(searchQuery, searchType);
            if (loadResult.LoadResultType == LavalinkLoadResultType.NoMatches || loadResult.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                return default;
            }
            if (loadResult.Tracks != null && !loadResult.Tracks.Any())
            {
                return default;
            }
            return loadResult.Tracks.FirstOrDefault();
        }

        private async Task<(LavalinkTrack, LavalinkSearchTypeInt)> GetTracksFromActivePlaylist(KeyValuePair<LavalinkNodeConnection, string> parameters)
        {
            LavalinkTrack musicTrack;
            var guildConfig = await _unitOfWork.GuildMusicData.GetByDiscordIdAsync(parameters.Value, x => x.ActivePlaylist);
            if (guildConfig.ActivePlaylist.Count <= 0) { return default; }
            var searchQuery = await parameters.Key.Rest.GetTracksAsync(guildConfig.ActivePlaylist[0].Uri.ToString(), LavalinkSearchType.Plain);
            if ((searchQuery.LoadResultType != LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType != LavalinkLoadResultType.LoadFailed) && searchQuery.Tracks != null && searchQuery.Tracks.Any())
            {
                musicTrack = searchQuery.Tracks.First();
                await _unitOfWork.GuildMusicData.DeleteFromPlaylist(guildConfig.ActivePlaylist[0]);
                return (musicTrack, LavalinkSearchTypeInt.Plain);
            }
            return default;
        }
        [SearchMethodAttribute(SearchType.AutoPlayDefaultTracks)]
        private async Task<(LavalinkTrack, LavalinkSearchTypeInt)> GetDefaultTracks(KeyValuePair<LavalinkNodeConnection, string> parameters)
        {
            LavalinkTrack track;
            var defaultPlaylists = ReaderJson.DeserializeFile<List<string>>("DefaultPlaylists");
            var searchQuery = await parameters.Key.Rest.GetTracksAsync(defaultPlaylists.RandomElement(), LavalinkSearchType.Plain);
            if ((searchQuery.LoadResultType != LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType != LavalinkLoadResultType.LoadFailed) && searchQuery.Tracks != null && searchQuery.Tracks.Any())
            {
                track = searchQuery.Tracks.RandomElement();
                return (track, LavalinkSearchTypeInt.Plain);
            }
            return default;
        }
        [SearchMethodAttribute(SearchType.AutoplayGuildPlaylists)]
        private async Task<(LavalinkTrack, LavalinkSearchTypeInt)> GetRandomTrackFromGuildPlaylist(KeyValuePair<LavalinkNodeConnection, string> parameters)
        {
            var tupleResponse = await _unitOfWork.GuildMusicData.GetRandomTrackFromGuildPlaylistAsync(parameters.Value);
            var searchQuery = await parameters.Key.Rest.GetTracksAsync(tupleResponse.track.Uri.ToString(), LavalinkSearchType.Plain);
            if ((searchQuery.LoadResultType != LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType != LavalinkLoadResultType.LoadFailed) && searchQuery.Tracks != null && searchQuery.Tracks.Any())
            {
                return (searchQuery.Tracks.FirstOrDefault(), LavalinkSearchTypeInt.Plain);
            }
            throw new Exception($"A track was retrieved from guild playlist but link no longer works.Playlist name:{tupleResponse.playlist.Name} Song Uri {tupleResponse.track.Uri}");
        }
        [SearchMethodAttribute(SearchType.AutoplayGuildTerm)]
        private async Task<(LavalinkTrack, LavalinkSearchTypeInt)> GetRandomTracksFromGuildSearchTerm(KeyValuePair<LavalinkNodeConnection, string> parameters)
        {
            var searchTerm = await _unitOfWork.GuildMusicData.GetRandomSearchTerm(parameters.Value);

            if (searchTerm.Term.Contains("youtube.com") || searchTerm.Term.Contains("soundcloud.com"))
            {
                var song = await GetTracksFromLavalinkAsync(parameters.Key, searchTerm.Term, LavalinkSearchType.Plain);
                return (song, LavalinkSearchTypeInt.Plain);
            }
            else
            {
                var track = await GetTracksFromLavalinkAsync(parameters.Key, searchTerm.Term, LavalinkSearchType.SoundCloud);
                if (track != null) { return (track, LavalinkSearchTypeInt.SoundCloud); }
                var song = await GetTracksFromLavalinkAsync(parameters.Key, searchTerm.Term, LavalinkSearchType.Youtube);
                return (song, LavalinkSearchTypeInt.Youtube);
            }
        }
    }
}

