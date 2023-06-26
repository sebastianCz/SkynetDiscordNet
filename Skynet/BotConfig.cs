using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
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
            services.AddScoped<ITest, Test>();
            services.AddScoped<ICrud, Crud>();
            services.AddScoped<IValidation, ValidateInput>();
            services.AddScoped<ISteamApiClient, SteamApiClient>();
            services.AddScoped<ICheaterLogic, CheaterLogic>();
            services.AddHttpClient(SteamApiConfig.SteamApiClientName,
               client => { client.BaseAddress = new Uri("https://api.steampowered.com"); });

            //Commands+ service provider config.

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false,
                Services = services.BuildServiceProvider()

            };

            var slash = Client.UseSlashCommands(new SlashCommandsConfiguration
            {
                Services = services.BuildServiceProvider()
            });
            Commands = Client.UseCommandsNext(commandsConfig);
            //commands registration
            Commands.RegisterCommands<PrefixCommands>();
            slash.RegisterCommands<CheaterSL>();

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
        private Task OnClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

    }
    public interface ITest
    {
        public int test { get; }
    }

    public class Test : ITest
    {
        public int test { get { return 2; } }
    }


}

