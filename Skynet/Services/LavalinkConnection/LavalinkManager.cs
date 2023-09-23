using ContosoUniversity.DAL;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Services.Interface;
using System.Collections.Concurrent;

namespace Skynet.Services.LavalinkConnection
{
    public class LavalinkManager : ILavalinkConnectionManager
    {
        private UnitOfWork _unitOfWork;
        public UnitOfWork UnitOfWork
        {
            get
            {

                if (this._unitOfWork == null)
                {
                    this._unitOfWork = new UnitOfWork();
                }
                return _unitOfWork;
            }
        }
        public LavalinkManager()
        {
            _unitOfWork = new UnitOfWork();
        }
        public async Task<(bool, string)> IsOnCooldown(InteractionContext ctx, ConcurrentDictionary<ulong, DateTime> guildsCooldowns, TimeSpan cooldownDuration)
        {
            var guildCooldown = guildsCooldowns.TryGetValue(ctx.Member.Guild.Id, out var lastTimeUsed);
            var remainingCooldown = cooldownDuration - (DateTime.UtcNow - lastTimeUsed);
            // Check if the user is on cooldown
            if (guildCooldown && DateTime.UtcNow - lastTimeUsed < cooldownDuration)
            {
                return (true, $"Command is on cooldown. Try again in {remainingCooldown.Seconds} seconds");
            }

            if (!guildCooldown) { guildsCooldowns.TryAdd(ctx.Member.Guild.Id, DateTime.UtcNow); }
            else { guildsCooldowns[ctx.Member.Guild.Id] = DateTime.UtcNow; }
            return (false, $"Command received. ");
            // Set the last time the user used the command

        }
        public async Task OnCommandChecksAsync(InteractionContext ctx)
        {
            var lavalinkExtension = ctx.Client.GetLavalink();
            ValidateLavalinkConnection(lavalinkExtension);
            await _unitOfWork.GuildMusicData.EnsureCreatedAsync(ctx.Guild.Id.ToString());
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
        }
        public async Task OnCommandChecksAsync(LavalinkExtension link, LavalinkNodeConnection node, LavalinkGuildConnection guild)
        {
            ValidateLavalinkConnection(link);
            await _unitOfWork.GuildMusicData.EnsureCreatedAsync(guild.Guild.Id.ToString());
            await _unitOfWork.SaveAsync();
            _unitOfWork.Dispose();
        }

        public async Task OnEventChecks(LavalinkExtension link, LavalinkNodeConnection node, LavalinkGuildConnection guild)
        {
            ValidateLavalinkConnection(link);
            await _unitOfWork.GuildMusicData.EnsureCreatedAsync(guild.Guild.Id.ToString());
            await _unitOfWork.SaveAsync();
        }

        /// <summary>
        /// Assure bot is connected after interaction from user( slash commands)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task AssureConnected(InteractionContext ctx)
        {
            if (!IsConnectedToVoice(ctx))
            {
                await ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value.ConnectAsync(ctx.Member.VoiceState.Channel);
            }
            return;
        }
        public async Task DisconnectAsync(InteractionContext ctx)
        {
            if (IsConnectedToVoice(ctx)) { await ctx.GetGuildConnection().DisconnectAsync(); }
        }
        /// <summary>
        /// Ensures bot is connected after an event is thrown ( in case this event was thrown after a disconnection)
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task AssureConnected(LavalinkGuildConnection guild, LavalinkNodeConnection node)
        {
            if (node.ConnectedGuilds.FirstOrDefault(x => x.Key == guild.Channel.Parent.Guild.Id).Value == null)
            {
                await node.ConnectAsync(guild.Channel);
            }
            return;
        }
        public bool IsConnectedToVoice(InteractionContext ctx)
        {
            return ctx.GetGuildConnection() != null;
        }
        public void ValidateVC(InteractionContext ctx)
        {
            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                throw new Exception("Please enter a voice channel");
            }
            if (ctx.Member.VoiceState.Channel.Type != DSharpPlus.ChannelType.Voice)
            {
                throw new Exception("Please enter a valid VC");
            }

        }
        public void ValidateLavalinkConnection(LavalinkExtension lavalinkExtension)
        {
            if (!lavalinkExtension.ConnectedNodes.Any())
            {
                throw new Exception($"Lavalink connection failed\n session bucket: {lavalinkExtension.Client.GatewayInfo.SessionBucket}");
            }
        }


    }
}
