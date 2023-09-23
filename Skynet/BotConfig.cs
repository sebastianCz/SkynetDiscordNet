using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Skynet.Commands;
using Skynet.db;
using Skynet.db.Repo;
using Skynet.Domain.GuildData;
using Skynet.Services.API;
using Skynet.Services.CommandHandlingLogic;
using Skynet.Services.Interface;
using Skynet.Services.LavalinkConnection;
using Skynet.Services.Search;

namespace Skynet
{
    public class BotConfig
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public BotConfig()
        {
        }

        public async Task RunAsync(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configuration["AppConfig:token"],
                TokenType = TokenType.Bot,
                AutoReconnect = true,

            };
            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)

            });

            //DI temporarly(hopefully) disabled due to DsharpPlus problems with DI 
            //Services registration
            var services = new ServiceCollection();
            services.AddScoped<IMusic, MusicService>();
            services.AddScoped<IMessageSender, MessageSender>();
            services.AddScoped<IYoutubeApiClient, YoutubeApiClient>();
            services.AddScoped<ILavalinkConnectionManager, LavalinkManager>();
            services.AddScoped<ISearchEngine, SearchEngine>();
            services.AddScoped<IGuildMusiDataRepo<GuildMusicData>, GuildMusicDataRepo>();
            var provider = services.BuildServiceProvider();
            var slash = Client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = provider

            });
            slash.RegisterCommands<MusicCommandHandler>();

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333,
                Secured = false,
            };
            var x = services.BuildServiceProvider();
            x.CreateScope();
            var lavaLinkConfig = new LavalinkConfiguration
            {
                Password = configuration["AppConfig:botPassword"],
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            // execute your command 
            var lavalinkHandlers = new LavalinkEventsHandlers();
            var lavalink = Client.UseLavalink();
            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavaLinkConfig);
            lavalink.ConnectedNodes.FirstOrDefault().Value.PlaybackFinished += (con, e) => lavalinkHandlers.PlaybackEnded(con, e);
            lavalink.ConnectedNodes.FirstOrDefault().Value.TrackStuck += (con, e) => lavalinkHandlers.TrackStuck(con, e);
            lavalink.ConnectedNodes.FirstOrDefault().Value.LavalinkSocketErrored += (con, e) => lavalinkHandlers.WebSocketClosed(con, e);
            lavalink.ConnectedNodes.FirstOrDefault().Value.TrackException += (con, e) => lavalinkHandlers.TrackException(con, e);
            lavalink.ConnectedNodes.FirstOrDefault().Value.PlaybackStarted += (con, e) => lavalinkHandlers.PlaybackStarted(con, e);
            await DbConfig(args);
            await Task.Delay(-1);
        }
        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
        [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
        public class RequireYearAttribute : CheckBaseAttribute
        {
            public int AllowedYear { get; private set; }

            public RequireYearAttribute(int year)
            {
                AllowedYear = year;
            }

            public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
            {
                return Task.FromResult(AllowedYear == DateTime.Now.Year);
            }
        }

        public async Task DbConfig(string[] arg)
        {
            var context = new BotContext();
            context.Database.Migrate();
            await context.SaveChangesAsync();
            context.Dispose();
        }

    }
}

