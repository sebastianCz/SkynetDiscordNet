using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Skynet.Domain.Enum;
using Skynet.Services.CommandHandlingLogic;
using Skynet.Services.LavalinkConnection;

namespace Skynet.Commands
{

    public class MusicCommandHandler : ApplicationCommandModule
    {
        private readonly MusicService _music;
        private readonly LavalinkManager _connectionManager;
        private readonly MessageSender _messageSender;
        public MusicCommandHandler()
        {
            _music = new MusicService();
            _messageSender = new MessageSender();
            _connectionManager = new LavalinkManager();
        }
        [SlashCommand("MusicReport", "Creates and sends a  music report based on the most played music authors etc")]
        [SlashCooldown(1, 2)]
        public async Task MusicReport(InteractionContext ctx)
        {
            try
            {
                await _connectionManager.OnCommandChecksAsync(ctx);
                _music.SendMusicReport(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        //[Command("generic"), RequireYear(2030)]
        [SlashCommand("CustomPlaylistCreate", "Creates a custom playlist and saves it")]
        [SlashCooldown(1, 2)]
        public async Task CustomPlaylistCreate(InteractionContext ctx, [Option("Query", "Provide a link or search term")] string playlistLink)
        {
            try
            {

                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.CreateCustomPlaylist(ctx, playlistLink);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        [SlashCommand("CustomPlaylistRemove", "Removes a playlist with the provided ID")]
        [SlashCooldown(1, 2)]
        public async Task CustomPlaylistRemove(InteractionContext ctx, [Option("Query", "Provide a link or search term")] string playlistId)
        {
            try
            {
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.RemoveCustomPlaylist(ctx, playlistId);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        [SlashCommand("CustomPlaylistsShow", "Shows existing playlists details")]
        [SlashCooldown(1, 2)]
        public async Task CustomPlaylistsShow(InteractionContext ctx)
        {
            try
            {
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.ShowCustomPlaylist(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }
        }
        [SlashCommand("Play", "Plays the specified music. You can provide a link or a search term")]
        [SlashCooldown(1, 2)]
        public async Task PlayMusic(InteractionContext ctx, [Option("Query", "Provide a link or search term")] string searchTerm)
        {
            try
            {

                _connectionManager.ValidateVC(ctx);
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _connectionManager.AssureConnected(ctx);
                await _music.PlayMusic(ctx, searchTerm);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        [SlashCommand("Resume", "Resumes music")]
        [SlashCooldown(1, 2)]
        public async Task ResumeMusic(InteractionContext ctx)
        {
            try
            {
                _connectionManager.ValidateVC(ctx);
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
        [SlashCommand("Stop", "Stops music and sets autoplay to off")]
        [SlashCooldown(1, 2)]
        public async Task StopMusic(InteractionContext ctx)
        {
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
        [SlashCooldown(1, 2)]
        public async Task PauseMusic(InteractionContext ctx)
        {
            try
            {
                _connectionManager.ValidateVC(ctx);
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
        [SlashCooldown(1, 2)]
        public async Task Skip(InteractionContext ctx)
        {
            try
            {
                _connectionManager.ValidateVC(ctx);
                await _connectionManager.OnCommandChecksAsync(ctx);


                await _music.Skip(ctx);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }
        [SlashCommand("Clear", "Clears Playlist")]
        [SlashCooldown(1, 2)]
        public async Task Clear(InteractionContext ctx)
        {
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
        [SlashCommand("PlayAuto", "Finds random songs based on user preferences and known tracks")]
        [SlashCooldown(1, 2)]
        public async Task PlayAuto(InteractionContext ctx, [Option("ShuffleStatus", "Type 1 to activate. Type 2 to deactivate")] string options)
        {
            try
            {
                _connectionManager.ValidateVC(ctx);
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.Autoplay(ctx, options);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }

        [SlashCommand("PlaylistShow", "Provides currently queued tacks")]
        [SlashCooldown(1, 2)]
        public async Task ShowPlaylist(InteractionContext ctx, [Option("DetailLevel", "Type 1 to get a summary. Type 2 to get full details")] string options)
        {
            try
            {
                await _connectionManager.OnCommandChecksAsync(ctx);
                await _music.ShowPlaylist(ctx, options);
            }
            catch (Exception e)
            {
                await _messageSender.SendMessageAsync(ctx, "An Error occured", e.Message, DiscordColor.Red);
                await _messageSender.LogError(ctx, e.Message, e.StackTrace, LoggingLevel.information);
            }

        }


    }
}