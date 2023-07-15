using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Skynet.Domain;
using Skynet.Services.Interface;

namespace Skynet.Services
{
    public class MessageSender : IMessageSender
    {
        public async Task LogError(InteractionContext ctx, string title, string description, LoggingLevel level)
        {
            var channel = await ctx.Client.GetChannelAsync(1122915800320319538);
            switch (level)
            {
                case LoggingLevel.information:
                    await SendMessage(title, description, channel, DiscordColor.Yellow);
                    break;
                case LoggingLevel.warning:
                    await SendMessage(title, description, channel, DiscordColor.Red);
                    break;
                default:
                    return;
            }
        } 
        public async Task SendMessage(string title, string description, DiscordChannel channel, DiscordColor color)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = title,
                Description = description,
                Color = color
            };

            await channel.SendMessageAsync(embedMessage);
        }
        public async Task SendMessageAsync(InteractionContext ctx, string title, string description, DiscordColor color)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = title,
                Description = description,
                Color = color
            };

            await ctx.Channel.SendMessageAsync(embedMessage);
        }
    }
}
