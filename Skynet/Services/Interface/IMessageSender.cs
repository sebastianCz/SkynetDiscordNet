﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Skynet.Domain;

namespace Skynet.Services.Interface
{
    public interface IMessageSender
    {
        public Task LogError(InteractionContext ctx, string title, string description, LoggingLevel level);


        public Task SendMessage(string title, string description, DiscordChannel channel, DiscordColor color);

        public Task SendMessageAsync(InteractionContext ctx, string title, string description, DiscordColor color);
       
    }
}
