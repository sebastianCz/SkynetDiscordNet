using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Skynet.Domain.Steam;

namespace Skynet.Commands
{
    public class PrefixCommands : BaseCommandModule
    {

        public ITest _test;
        public IHttpClientFactory _clientFactory;
        [Command("test")]
        public async Task TestCommand(CommandContext ctx)
        {
            var steamProfileLink =
                "https://api.steampowered.com" +
                "/ISteamUser" +
                "/GetPlayerSummaries" +
                "/v0002" +
                "/?key=AB38F06FC3060B43D9160B098E816854&steamids=76561198013792223";

            var steamProfileLink2 = "https://steamcommunity.com/id/VikkoxX/";

            var x = _test.test;
            string responseBody;
            try
            {
            http://api.steampowered.com/ISteamUser/ResolveVanityURL/v0001/?key=AB38F06FC3060B43D9160B098E816854&vanityurl=userVanityUrlName
                var response = await _clientFactory.CreateClient(SteamApiConfig.SteamApiClientName).GetAsync($"{steamProfileLink2}");
                responseBody = await response.Content.ReadAsStringAsync();
                var deserialized = JsonConvert.DeserializeObject<SteamPlayerList>(responseBody);

            }
            catch
            {
                throw new Exception("Failed to fetch API data");
            }
            var embedMessage = new DiscordMessageBuilder()
               .AddEmbed(new DiscordEmbedBuilder()
               .WithTitle("Test title")
               .WithDescription("Hardcoded steam api results" + responseBody)
               .WithColor(DiscordColor.Green)
               );
            await ctx.Channel.SendMessageAsync(embedMessage);

        }


    }
}
