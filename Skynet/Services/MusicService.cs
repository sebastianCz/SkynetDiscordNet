using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Services.Interface;

namespace Skynet.Services
{
    public class MusicService : IMusic
    {
        private readonly IMessageSender _messageSender;
        public MusicService(IMessageSender sender)
        {
                _messageSender = sender;
        }
        public async Task PlayMusic(InteractionContext ctx, string query)
        {
            var lavalink = await ValidateUserAndConnection(ctx);
            var node = lavalink.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            var searchQuery = await node.Rest.GetTracksAsync(query); 
            if (searchQuery.LoadResultType == LavalinkLoadResultType.NoMatches || searchQuery.LoadResultType == LavalinkLoadResultType.LoadFailed)
            {
                throw new Exception("Failed to find music corresponding to your query");
            } 
            var musicTrack = searchQuery.Tracks.First(); 
            _messageSender.SendMessage(ctx, $"Now Playing: {musicTrack.Title}", $" Link: {musicTrack.Uri}",DiscordColor.Green);
            await conn.PlayAsync(musicTrack);  
        }
        public async Task PauseMusic(InteractionContext ctx)
        {
            var lavalink = await ValidateUserAndConnection(ctx);
            var node = lavalink.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            await conn.PauseAsync();  
            _messageSender.SendMessage(ctx, $"Track Paused", $" Type /Resume to keep listening", DiscordColor.Yellow); 
        }
        public async Task ResumeMusic(InteractionContext ctx)
        {
            var lavalink = await ValidateUserAndConnection(ctx);
            var node = lavalink.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            await conn.ResumeAsync();
            _messageSender.SendMessage(ctx, $"Track Resumed", $" Music should be now playing", DiscordColor.Green);
        }
        public async Task StopMusic(InteractionContext ctx)
        {
            var lavalink = await ValidateUserAndConnection(ctx);
            var node = lavalink.ConnectedNodes.Values.First();
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            await conn.StopAsync();
            await conn.DisconnectAsync();
            _messageSender.SendMessage(ctx, $"Music stopped", $" Turns out you can stop Rock'N'Roll", DiscordColor.Red);
        }

        private async Task<LavalinkExtension> ValidateUserAndConnection(InteractionContext ctx)
        {
            var lavaLinkInstance = ctx.Client.GetLavalink(); //LavalinkExtension 
            var userVc = ctx.Member.VoiceState.Channel;

            if (ctx.Member.VoiceState == null || userVc == null)
            {
                throw new Exception("Please enter a voice channel");
            }
            if (!lavaLinkInstance.ConnectedNodes.Any())
            {
                throw new Exception("Lavalink connection failed");
            }
            if (userVc.Type != DSharpPlus.ChannelType.Voice)
            {
                throw new Exception("Please enter a valid VC");
            }

            var node = lavaLinkInstance.ConnectedNodes.Values.First();
            //LavalinkNodeConnection

            await node.ConnectAsync(userVc);
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);

            if (conn == null)
            {
                throw new Exception("lavalink failed to connect");
            }
            return lavaLinkInstance;

        }
         
    }
}
