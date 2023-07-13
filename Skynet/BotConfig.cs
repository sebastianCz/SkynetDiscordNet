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
            services.AddHttpClient(SteamApiConfig.SteamApiClientName,
               client => { client.BaseAddress = new Uri("https://api.steampowered.com"); });

            //Commands+ service provider config.

            //var commandsConfig = new CommandsNextConfiguration()
            //{
            //    StringPrefixes = new string[] { configJson.Prefix },
            //    EnableMentionPrefix = true,
            //    EnableDms = true,
            //    EnableDefaultHelp = false,
            //    Services = services.BuildServiceProvider()

            //};
            //
            // Commands = Client.UseCommandsNext(commandsConfig);
            //
            //Commands.RegisterCommands<PrefixCommands>();

            var slash = Client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = services.BuildServiceProvider()
            });
            //commands registration
            //slash.RegisterCommands<CheaterSL>();
            slash.RegisterCommands<Music>();

            var endpoint = new ConnectionEndpoint
            {
                Hostname = "lavalink.snooby.ml",
                Port = 443,
                Secured = true,

            };
            var lavaLinkConfig = new LavalinkConfiguration
            {
                Password = "discord.gg/6xpF6YqVDd",
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint
            };
            var lavalink = Client.UseLavalink(); 

            await Client.ConnectAsync();
            await lavalink.ConnectAsync(lavaLinkConfig) ;
            await Task.Delay(-1);
        }
        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }  
    }  
}

