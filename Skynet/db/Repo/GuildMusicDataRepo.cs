using Microsoft.EntityFrameworkCore;
using Skynet.Domain;
using Skynet.Domain.GuildData;
using Skynet.Repository.Repository;
using Skynet.Services;
using System.Linq.Expressions;

namespace Skynet.db.Repo
{
    public class GuildMusicDataRepo : EntityBaseRepo<GuildMusicData>, IGuildMusiDataRepo<GuildMusicData>
    {
        private BotContext _context;
        public GuildMusicDataRepo(BotContext context) : base(context)
        {
            _context = context;
        }
        /// <summary>
        /// This one is used on each method as a user can enter whatever command first but we will still need him created for most search cases. 
        ///Can be moved to only methods really needing it in case things go slow. 
        /// </summary>
        /// <param name="guildId"></param>
        /// <returns></returns>
        public async Task<GuildMusicData> EnsureCreatedAsync(string guildId)
        {
            var guildConfig = await this.GetByDiscordIdAsync(guildId);
            if (guildConfig == null)
            {
                var probability = new SearchProbability();
                guildConfig = new GuildMusicData()
                {
                    DiscordId = guildId,
                    AutoplayOn = true,
                    SearchProbability = probability
                };
                await this.AddAsync(guildConfig);
            }
            return guildConfig;
        }
        public async Task<GuildMusicReport> GenerateGuildReportAsync(int guildId)
        {
            var query = _context.Results
               .GroupBy(searchEngineResult => searchEngineResult.GuildMusicDataId)
               .Where(x => x.Key == guildId)
               .Select(group => new
               {
                   TotalCount = group.Count(),
                   MostCommonLavalinkSearchType = group.GroupBy(result => result.LavalinkSearchType)
                                                     .OrderByDescending(g => g.Count())
                                                     .Select(g => g.Key)
                                                     .FirstOrDefault(),
                   MostCommonSearchType = group.GroupBy(result => result.SearchType)
                                                .OrderByDescending(g => g.Count())
                                                .Select(g => g.Key)
                                                .FirstOrDefault(),
                   MostCommonSearchInput = group.GroupBy(result => result.SearchInput)
                                                 .OrderByDescending(g => g.Count())
                                                 .Select(g => g.Key)
                                                 .FirstOrDefault(),
                   MostCommonSongName = group.GroupBy(result => result.SongName)
                                             .OrderByDescending(g => g.Count())
                                             .Select(g => g.Key)
                                             .FirstOrDefault(),
                   MostCommonSongUri = group.GroupBy(result => result.SongUri)
                                            .OrderByDescending(g => g.Count())
                                            .Select(g => g.Key)
                                            .FirstOrDefault()
               });
            var results = query.ToList();
            var report = new GuildMusicReport();
            report.SearchesCount = results[0].TotalCount;
            report.MostCommonLavalinkSearchType = results[0].MostCommonLavalinkSearchType;
            report.MostCommonSearchPattern = results[0].MostCommonSearchType;
            report.MostCommonSearchInput = results[0].MostCommonSearchInput;
            report.MostCommonSongName = results[0].MostCommonSongName;
            report.MostCommonSongUri = results[0].MostCommonSongUri;
            return report;
        }
        public async Task<GuildMusicData> GetActivePlaylistAsync(string id)
        {
            var projection = await _context.GuildMusicDatas
                                  .Where(c => c.DiscordId == id)
                                  .Select(x => new
                                  {
                                      guildData = x,
                                      tracks = x.ActivePlaylist.Where(h => h.GuildMusicDataId == x.Id)
                                  }).SingleOrDefaultAsync();
            return projection.guildData;
        }
        public async Task<GuildMusicData> GetByDiscordIdAsync(string id, params Expression<Func<GuildMusicData, object>>[] includeProperties)
        {
            IQueryable<GuildMusicData> query = _context.Set<GuildMusicData>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.SingleOrDefaultAsync(x => x.DiscordId == id);
        }

        public async Task<GuildMusicData> GetByDiscordIdAsync(string id)
        {
            var result = await _context.GuildMusicDatas.SingleOrDefaultAsync(x => x.DiscordId == id);
            return result;
        }
        public async Task ClearActivePlaylist(string discordId)
        {
            var guildMusic = await GetActivePlaylistAsync(discordId);
            _context.RemoveRange(guildMusic.ActivePlaylist);
            return;
        }
        public async Task DeleteFromPlaylist(LavalinkTrackBot song)
        {
            _context.Remove(song);
            return;
        }

        public async Task DeletePlaylist(int id)
        {
            var playlist = await _context.Playlists
                 .Include(x => x.Playlist)
                 .SingleOrDefaultAsync(x => x.Id == id);
            _context.RemoveRange(playlist.Playlist);
            _context.Remove(playlist);
        }
        public async Task<(MusicPlaylist playlist, LavalinkTrackBot track)> GetRandomTrackFromGuildPlaylistAsync(string id)
        {
            var rand = new Random();
            var guildMusicData = await _context.GuildMusicDatas
                .Include(x => x.Playlists)
                .FirstOrDefaultAsync(x => x.DiscordId == id);
            var playlist = guildMusicData.Playlists.RandomElement();
            var playlistWithInclude = _context.Playlists
                .Include(x => x.Playlist)
                .Where(x => x.Id == playlist.Id)
                .SingleOrDefault();
            var song = playlistWithInclude.Playlist.RandomElement();
            return (playlist, song);
        }
        public async Task<MusicSearchTerm> GetRandomSearchTerm(string id)
        {
            var rand = new Random();
            var guildMusicData = await _context.GuildMusicDatas
                .Include(x => x.SearchTerms)
                .FirstOrDefaultAsync(x => x.DiscordId == id);
            var searchTerm = guildMusicData.SearchTerms.RandomElement();
            return searchTerm;
        }
        public async Task AddSearchResult(GuildMusicData data, SearchEngineResult result)
        {
            result.GuildMusicDataId = data.Id;
            await _context.Results.AddAsync(result);
        }
    }
}
