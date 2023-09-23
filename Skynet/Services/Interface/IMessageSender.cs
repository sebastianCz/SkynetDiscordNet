using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Domain.Enum;

namespace Skynet.Services.Interface
{
    public interface IMessageSender
    {
        public Task LogError(InteractionContext ctx, string title, string description, LoggingLevel level, DiscordChannel discordChannel = null); 

        public Task SendMessageAsync(string title, string description, DiscordChannel channel, DiscordColor color);

        public Task SendMessageAsync(InteractionContext ctx, string title, string description, DiscordColor color);
       
    }
}
