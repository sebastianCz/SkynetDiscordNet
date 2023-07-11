using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.Services.Interface;
using System.Xml.Linq;

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
            var lavalink = await ValidateUser(ctx);
            var node = lavalink.ConnectedNodes.Values.First();   
            await node.ConnectAsync(ctx.Member.VoiceState.Channel);
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);  
            if (conn == null)
            {
                throw new Exception("lavalink failed to connect");
            }

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
            var lavalink = await ValidateUser(ctx);
            var node = lavalink.ConnectedNodes.Values.First();
            var conn = await IsBotConnected(ctx, node); 
                await conn.PauseAsync();
                _messageSender.SendMessage(ctx, $"Track Paused", $" Type /Resume to keep listening", DiscordColor.Yellow); 
            
        }
        public async Task ResumeMusic(InteractionContext ctx)
        {
            var lavalink = await ValidateUser(ctx);
            var node = lavalink.ConnectedNodes.Values.First();
            var conn = await IsBotConnected(ctx, node);
            await conn.ResumeAsync();
            _messageSender.SendMessage(ctx, $"Track Resumed", $" Music should be now playing", DiscordColor.Green);
        }
        public async Task StopMusic(InteractionContext ctx)
        {
            var lavalink = await ValidateUser(ctx);
            var node = lavalink.ConnectedNodes.Values.First();
            var conn = await IsBotConnected(ctx, node);

            await conn.StopAsync();
            await conn.DisconnectAsync();
            _messageSender.SendMessage(ctx, $"Music stopped", $" Turns out you can stop Rock'N'Roll", DiscordColor.Red);
        }

        private async Task<LavalinkExtension> ValidateUser(InteractionContext ctx)
        {
            var lavaLinkInstance = ctx.Client.GetLavalink(); //LavalinkExtension  

            if (ctx.Member.VoiceState == null || ctx.Member.VoiceState.Channel == null)
            {
                throw new Exception("Please enter a voice channel");
            }
            if (!lavaLinkInstance.ConnectedNodes.Any())
            {
                throw new Exception("Lavalink connection failed");
            }
            if (ctx.Member.VoiceState.Channel.Type != DSharpPlus.ChannelType.Voice)
            {
                throw new Exception("Please enter a valid VC");
            } 
           
            return lavaLinkInstance;

        } 
        private async Task<LavalinkGuildConnection> IsBotConnected(InteractionContext ctx,LavalinkNodeConnection node)
        {
            var conn = node.GetGuildConnection(ctx.Member.VoiceState.Guild);
            if (conn == null) { throw new Exception("I'm not in a channel right now."); }
            return conn;
        } 
    }
}
