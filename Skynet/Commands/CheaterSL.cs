using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Skynet.db;
using Skynet.Domain;
using Skynet.Services.Interface;

namespace Skynet.Commands
{
    public class CheaterSL : ApplicationCommandModule
    {
        private readonly ICrud _crud;
        private readonly ICheaterLogic _manageCheaters;
        private readonly IMessageSender _messageSender;
        public CheaterSL(ICrud crud, ICheaterLogic verifyUser)
        {
            _crud = crud;
            _manageCheaters = verifyUser;
        }

        [SlashCommand("DisplayAllCheaters", "Displays a list of all known cheaters")]
        public async Task DisplayAllCheaters(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(
                  InteractionResponseType.ChannelMessageWithSource,
                  new DiscordInteractionResponseBuilder()
                  .WithContent("Verify Command received"));

            try
            {
                var search = await _manageCheaters.DisplayAll();
                await _messageSender.SendMessage(ctx, search.Title, search.Description, DiscordColor.Green);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An error occured the operation", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.StackTrace, e.Source, LoggingLevel.information);
            }

        }
        [SlashCommand("Verify", "Verify if steam profile is known")]
        public async Task VerifyCommand(InteractionContext ctx, [Option("ProfileLink", "Provide a steam profile link")] string steamProfileLink)
        {
            await ctx.CreateResponseAsync(
                   InteractionResponseType.ChannelMessageWithSource,
                   new DiscordInteractionResponseBuilder()
                   .WithContent("Verify Command received"));
            try
            {
                var search = await _manageCheaters.IsCheater(steamProfileLink);
                if (search.Found)
                {
                    await _messageSender.SendMessage(ctx, "Cheater Detected", search.Description, DiscordColor.Red);
                }
                if (!search.Found && search.Found != null)
                {
                    await _messageSender.SendMessage(ctx, "User is legit", search.Description, DiscordColor.Green);
                }

            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An error occured the operation", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.StackTrace, e.Source, LoggingLevel.information);
                if (e.InnerException != null)
                {
                    await _messageSender.SendMessage(ctx, "Critical error", "A critical error occured. Check if steam is still working properly and try again later.", DiscordColor.Red);
                    await _messageSender.LogError(ctx, e.StackTrace, e.Source, LoggingLevel.critical);
                }

            }

        }

        [SlashCommand("Add", "Save user steam profile")]
        public async Task AddCommand(
        InteractionContext ctx,
        [Option("ProfileLink", "Provide a steam profile link")] string steamProfileLink)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {
                var search = await _manageCheaters.AddCheater(steamProfileLink, ctx.User.Username);
                if (search.Found)
                {

                    await _messageSender.SendMessage(ctx, search.Title, search.Description, DiscordColor.Green);
                }
                else
                {

                    await _messageSender.SendMessage(ctx, search.Title, search.Description, DiscordColor.Red);
                }
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "User not added", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }

        [SlashCommand("Delete", "Delete user profile from known users list")]
        public async Task DeleteCommand(
        InteractionContext ctx,
       [Option("ProfileLink", "Provide a steam profile link")] string steamProfileLink)
        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {
                var search = await _manageCheaters.DeleteCheater(steamProfileLink, ctx.User.Username);
                if (search.Found)
                {

                    await _messageSender.SendMessage(ctx, search.Title, search.Description, DiscordColor.Green);
                }
                else
                {

                    await _messageSender.SendMessage(ctx, search.Title, search.Description, DiscordColor.Red);
                }
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }

        [SlashCommand("Reset", "Deletes the list. It will be replaced by a new, empty list.")]
        public async Task Reset(
        InteractionContext ctx, [Option("AreYouSure", "TYPE DELETE. YOU CAN'T REVERSE THIS ACTION.")] string confirmation)

        {
            await ctx.CreateResponseAsync(
                InteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder()
                .WithContent("Commend received")
                );
            try
            {
                if (_crud.PurgeCheaters(ctx.Member.DisplayName, confirmation))
                {
                    await _messageSender.SendMessage(ctx, "List purged", "List purged successfully", DiscordColor.Green);
                }
                else
                {
                    await _messageSender.SendMessage(ctx, "Purge failure", "Impossible to purge. You might not have the right authorisation.\n Make sure the command was entered correctly.", DiscordColor.Red);
                }
            }
            catch (Exception e)
            {
                await _messageSender.SendMessage(ctx, "An error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }



        }
  

    }
}
