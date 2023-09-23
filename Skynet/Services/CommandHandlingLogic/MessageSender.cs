using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Domain.Enum;
using Skynet.Services.Interface;

namespace Skynet.Services.CommandHandlingLogic
{
    public class MessageSender : IMessageSender
    {
        public MessageSender()
        {

        }
        public async Task LogError(InteractionContext ctx, string title, string description, LoggingLevel level, DiscordChannel discordChannel = null)
        {
            DiscordChannel channel;
            if (ctx.Member != null)
            {
                //logs errors only on my private server, this contains stack trace
                var node = ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value;
                var guild = node.ConnectedGuilds.FirstOrDefault(x => x.Key == 430780143129067532).Value;
                if (guild != null) { channel = guild.Guild.Channels.FirstOrDefault(x => x.Key == 1122915800320319538).Value; }
                else
                {
                    channel = discordChannel;
                }
            }
            else
            {
                channel = discordChannel;
            }

            if (channel != null)
            {
                switch (level)
                {
                    case LoggingLevel.information:
                        await SendMessageAsync(title, description, channel, DiscordColor.Yellow);
                        break;
                    case LoggingLevel.warning:
                        await SendMessageAsync(title, description, channel, DiscordColor.Red);
                        break;
                    default:
                        return;
                }
            }

        }

        public async Task SendMessageAsync(string title, string description, DiscordChannel channel, DiscordColor color)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = title.Truncate(256),
                Description = description,
                Color = color
            };
            channel = await FindPlaylistOrDefault(channel.Guild.Channels);
            await channel.SendMessageAsync(embedMessage);
        }
        public async Task SendMessageAsync(InteractionContext ctx, string title, string description, DiscordColor color)
        {
            var embedMessage = new DiscordEmbedBuilder()
            {
                Title = title.Truncate(256),
                Description = description,
                Color = color
            };
            var channel = await FindPlaylistOrDefault(ctx.Guild.Channels);
            await channel.SendMessageAsync(embedMessage);
        }
        private async Task<DiscordChannel> FindPlaylistOrDefault(IReadOnlyDictionary<ulong, DiscordChannel> channelsList)
        {
            var playlistName = "playlist";
            var channelOrCategory = channelsList.FirstOrDefault(x => x.Value.Name.ToLower().Trim().Contains(playlistName)).Value;
            if (channelOrCategory != null && !channelOrCategory.IsCategory) { return channelOrCategory; }
            if (channelOrCategory != null && channelOrCategory.IsCategory)
            {
                var channel = channelOrCategory.Children.FirstOrDefault(x => x.Name.ToLower().Trim().Contains(playlistName));
                if (channel != null) { return channel; }
                channel = await channelsList.FirstOrDefault().Value.Guild.CreateChannelAsync(playlistName, DSharpPlus.ChannelType.Text);
                return channel;
            }
            if (channelOrCategory == null)
            {
                var channel = await channelsList.FirstOrDefault().Value.Guild.CreateChannelAsync(playlistName, DSharpPlus.ChannelType.Text);
                return channel;
            }
            return default;
        }

    }
}
