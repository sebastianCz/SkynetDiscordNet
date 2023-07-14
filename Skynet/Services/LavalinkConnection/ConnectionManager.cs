using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Domain;
using Skynet.Services.Interface;
using System.Reflection.Metadata.Ecma335;

namespace Skynet.Services.LavalinkConnection
{
    public class LavalinkConnectionManager : ILavalinkConnectionManager
    {
        public async Task OnCommandChecksAsync(InteractionContext ctx)
        {
            var lavalinkExtension = ctx.Client.GetLavalink();
            await ValidateUser(ctx); 
            await ValidateLavalinkConnection(ctx, lavalinkExtension); 
        }
       
        public async Task AssureConnected(InteractionContext ctx)
        {
            if (!await IsConnectedAsync(ctx))
            {
                await ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value.ConnectAsync(ctx.Member.VoiceState.Channel);
            }
            return;
        }
        public async Task<bool> IsConnectedAsync(InteractionContext ctx)
        {
            var guildConnection = ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value.ConnectedGuilds.FirstOrDefault(x => x.Key == ctx.Guild.Id);
            return guildConnection.Value != null; 
        }
        private async Task ValidateUser(InteractionContext ctx)
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
        private async Task ValidateLavalinkConnection(InteractionContext ctx, LavalinkExtension lavalinkExtension)
        {
            if (!lavalinkExtension.ConnectedNodes.Any())
            {
                throw new Exception("Lavalink connection failed");
            }
        }

        public async Task<LavalinkGuildConnection> GetGuildConnection(InteractionContext ctx)=> ctx.Client.GetLavalink().GetGuildConnection(ctx.Member.VoiceState.Guild);
        public async Task<LavalinkNodeConnection> GetNodeConnection(InteractionContext ctx) => ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value;
        public async Task <LavalinkConnectionData> GetConnectionData(InteractionContext ctx)
        {
            return new LavalinkConnectionData()
            {
                GuildConnection = await GetGuildConnection(ctx),
                NodeConnection = await GetNodeConnection(ctx)
            };
        }
    }
}
