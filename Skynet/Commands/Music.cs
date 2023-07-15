using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Skynet.Domain;
using Skynet.Services.Interface;

namespace Skynet.Commands
{
    public class Music : ApplicationCommandModule
    {
        private readonly IMusic _music;
        private readonly ILavalinkConnectionManager _connectionManager;
        private readonly IMessageSender _messageSender;
        public Music(IMusic crud,IMessageSender sender,ILavalinkConnectionManager connectionManager)
        {
            _music  = crud;
            _messageSender = sender;    
            _connectionManager = connectionManager;
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
                await _connectionManager.AssureConnected(ctx);
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.PlayMusic(ctx, searchTerm); 
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
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
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _connectionManager.AssureConnected(ctx);
                await _music.ResumeMusic(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
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
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.StopMusic(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
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
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.PauseMusic(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }
             
        }
        [SlashCommand("Skip", "Skips to next track on playlist")]
        public async Task Skip(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            { 
                await _connectionManager.OnCommandChecksAsync(ctx); 
                await _connectionManager.AssureConnected(ctx);
                await _music.Skip(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        [SlashCommand("Clear", "Clears Playlist")]
        public async Task Clear(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.Clear(ctx);
                await _messageSender.SendMessageAsync(ctx, "Playlist cleared", "No tracks queued", DiscordColor.Red);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        [SlashCommand("Shuffle", "Finds random songs based on user preferences and known tracks")]
        public async Task Shuffle(InteractionContext ctx, [Option("ShuffleStatus","Type 1 to activate. Type 2 to deactivate")]string options)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _connectionManager.AssureConnected(ctx); 
                await _music.Autoplay(ctx,options);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        
        [SlashCommand("ShowPlaylist", "Provides currently queued tacks")]
        public async Task ShowPlaylist(InteractionContext ctx, [Option("DetailLevel", "Type 1 to get a summary. Type 2 to get full details")] string options)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.ShowPlaylist(ctx,options);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }


    }
}