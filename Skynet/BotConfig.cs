using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using Skynet.Commands;
using Skynet.db;
using Skynet.Domain;
using Skynet.Domain.Steam;
using Skynet.Services;
using Skynet.Services.Interface;
using Skynet.Services.LavalinkConnection;
using System.Diagnostics;
using System.Numerics;

namespace Skynet
{
    public class BotConfig
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }

        public async Task RunAsync()
        { 
            var configString = ReaderJson.ReadFile("config");
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(configString);
            var config = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                
            };
            Client = new DiscordClient(config);
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
                
            });
             



            //Services registration
            var services = new ServiceCollection(); 
            services.AddScoped<ICrud, Crud>();
            services.AddScoped<IValidation, ValidateInput>();
            services.AddScoped<ISteamApiClient, SteamApiClient>();
            services.AddScoped<ICheaterLogic, CheaterLogic>();
            services.AddScoped<IMusic, MusicService>();
            services.AddScoped<IMessageSender, MessageSender>(); 
            services.AddScoped<ILavalinkConnectionManager, LavalinkConnectionManager>();
            services.AddScoped<ISearchEngine, SearchEngine>();
            
            services.AddHttpClient(SteamApiConfig.SteamApiClientName,
               client => { client.BaseAddress = new Uri("https://api.steampowered.com"); });
             
            var slash = Client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = services.BuildServiceProvider()
            });
            //commands registration
            //slash.RegisterCommands<CheaterSL>();
            slash.RegisterCommands<Music>();

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "127.0.0.1",
                Port = 2333,
                Secured = false,

            };
            var lavaLinkConfig = new LavalinkConfiguration
            {
                Password = "youshallnotpass",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };

            // execute your command

            var lavalinkHandlers = new LavalinkEventsHandlers();
            var lavalink = Client.UseLavalink();

            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavaLinkConfig); 
            lavalink.ConnectedNodes.FirstOrDefault().Value.PlaybackStarted += (con, e) => lavalinkHandlers.PlaybackStarted(con, e);
            lavalink.ConnectedNodes.FirstOrDefault().Value.PlaybackFinished += (con, e) => lavalinkHandlers.PlaybackEnded(con, e);
            await Task.Delay(-1);
        }
        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
      
    }  
}

