﻿using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Skynet.Domain;
using Skynet.Services.Interface;

namespace Skynet.Commands
{
    public class Music : ApplicationCommandModule
    {
        private readonly IMusic _music;
        private readonly IMessageSender _messageSender;
        public Music(IMusic crud,IMessageSender sender)
        {
            _music  = crud;
            _messageSender = sender;    
        }

        [SlashCommand("Play", "Plays the specified music. You can provide a link or a search term")]
        public async Task PlayMusic(InteractionContext ctx, [Option("Query", "Provide a link or search term")] string searchTerm)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {
                await _music.PlayMusic(ctx, searchTerm); 
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }
            
        }
        [SlashCommand("Resume", "Resumes music")]
        public async Task ResumeMusic(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {

                await _music.ResumeMusic(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }
        }
        [SlashCommand("Stop", "Stops music")]
        public async Task StopMusic(InteractionContext ctx )
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {

                await _music.StopMusic(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }
           
        }
        [SlashCommand("Pause", "Pauses music")]
        public async Task PauseMusic(InteractionContext ctx )
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            { 
                await _music.PauseMusic(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }
             
        }
    }
}