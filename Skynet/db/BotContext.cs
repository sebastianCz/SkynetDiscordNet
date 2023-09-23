using DSharpPlus.Lavalink;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Skynet.Domain;
using Skynet.Domain.GuildData;

namespace Skynet.db
{
    public class BotContext : DbContext
    {

        public DbSet<GuildMusicData> GuildMusicDatas { get; set; }
        public DbSet<MusicSearchTerm> SearchTerms { get; set; }
        public DbSet<MusicUser> MusicUsers { get; set; }
        public DbSet<MusicPlaylist> ActivePlaylist { get; set; }
        public DbSet<MusicPlaylist> Playlists { get; set; }
        public DbSet<MusicSearchTerm> LavalinkTracks { get; set; }
        public DbSet<SearchProbability> Probabilities { get; set; }
        public DbSet<SearchEngineResult> Results { get; set; }

        public BotContext()
        {
        }

        public BotContext(DbContextOptions<BotContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            base.OnModelCreating(builder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(GetConnectionString());
        }
        private  string GetConnectionString()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
            var connectionString = configuration.GetConnectionString("DiscordBot");
            return connectionString;
        }
        private void ConfigureRelations(ModelBuilder builder)
        {

            builder.Entity<GuildMusicData>()
                .HasOne(x => x.SearchProbability)
                .WithOne(x => x.GuildMusicData);

            builder.Entity<GuildMusicData>()
                .HasMany(x => x.SearchTerms)
                .WithOne(x => x.GuildMusicData)
                .HasForeignKey(x => x.GuildMusicDataId) 
        .HasPrincipalKey(x => x.Id);

            builder.Entity<GuildMusicData>()
             .HasMany(x => x.Playlists)
             .WithOne(y => y.GuildMusicData)
             .HasForeignKey(y => y.GuildMusicDataId) 
        .HasPrincipalKey(x => x.Id);
            builder.Entity<GuildMusicData>()
            .HasMany(x => x.ActivePlaylist)
            .WithOne(y => y.GuildMusicData)
            .HasForeignKey(y => y.GuildMusicDataId) 
        .HasPrincipalKey(x => x.Id); 
            builder.Entity<MusicPlaylist>()
          .HasMany(x => x.Playlist)
          .WithOne(y => y.MusicPlaylist)
          .HasForeignKey(y => y.GuildMusicDataId)
          .OnDelete(DeleteBehavior.Cascade) 
        .HasPrincipalKey(x => x.Id);
            builder.Entity<MusicUser>()
         .HasMany(x => x.SearchTerms)
         .WithOne(y => y.MusicUser)
         .HasForeignKey(y => y.MusicUserId) 
        .HasPrincipalKey(x => x.Id);
            builder.Entity<GuildMusicData>()
        .HasMany(x => x.SearchEngines)
        .WithOne(y => y.GuildMusicData)
        .HasForeignKey(x => x.GuildMusicDataId)
        .HasPrincipalKey(x => x.Id);
         
        }
    }  
 } 
