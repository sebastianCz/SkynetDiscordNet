using DSharpPlus.Lavalink;
using DSharpPlus.SlashCommands;
using Skynet.db;
using Skynet.Domain;
using Skynet.Services.Interface;
using System;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

namespace Skynet.Services.LavalinkConnection
{
    public class LavalinkConnectionManager : ILavalinkConnectionManager
    {
        public async Task OnCommandChecksAsync(InteractionContext ctx)
        {
            var lavalinkExtension = ctx.Client.GetLavalink();
            var connectionData = await GetConnectionData(ctx); 
            await ValidateUser(ctx); 
            await ValidateLavalinkConnection(lavalinkExtension); 
            await EnsureConfigFileExists(ctx,connectionData.NodeConnection, connectionData.GuildConnection);
        }
        public async Task OnCommandChecksAsync(LavalinkExtension link, LavalinkNodeConnection node, LavalinkGuildConnection guild)
        { 
            await ValidateLavalinkConnection(link);
            await EnsureConfigFileExists(node, guild);
        }
        private async Task EnsureConfigFileExists(LavalinkNodeConnection node,LavalinkGuildConnection guild)
        {
            var musicConfig= ReaderJson.DeserializeFile<List<MusicConfig>>("MusicConfig");
            
            var guildConfig = musicConfig.Where(x => x.Id == guild.Guild.Id.ToString());
            if (!guildConfig.Any())
            {
                var newConfig = new MusicConfig()
                { 
                    Id = guild.Guild.Id.ToString(),
                    AutoplayOn = true
                };
                musicConfig.Add(newConfig);
                ReaderJson.SerialiseAndSave(musicConfig, "MusicConfig");
            }
            
        }
        private async Task EnsureConfigFileExists(InteractionContext ctx,LavalinkNodeConnection node, LavalinkGuildConnection guild)
        {
            var musicConfig = ReaderJson.DeserializeFile<List<MusicConfig>>("MusicConfig");

            var guildConfig = musicConfig.Where(x => x.Id == ctx.Member.Guild.Id.ToString());
            if (!guildConfig.Any())
            {
                var newConfig = new MusicConfig()
                {
                    Id = guild.Guild.Id.ToString(),
                    AutoplayOn = true
                };
                musicConfig.Add(newConfig);
                ReaderJson.SerialiseAndSave(musicConfig, "MusicConfig");
            }

        }


        /// <summary>
        /// Assure bot is connected after interaction from user( slash commands)
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public async Task AssureConnected(InteractionContext ctx)
        {
            if (!await IsConnectedAsync(ctx))
            {
                await ctx.Client.GetLavalink().ConnectedNodes.FirstOrDefault().Value.ConnectAsync(ctx.Member.VoiceState.Channel);
            }
            return;
        }
        /// <summary>
        /// Ensures bot is connected after an event is thrown ( in case this event was thrown after a disconnection)
        /// </summary>
        /// <param name="guild"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task AssureConnected(LavalinkGuildConnection guild,LavalinkNodeConnection node)
        {
            if (node.ConnectedGuilds.FirstOrDefault(x => x.Key == guild.Channel.Parent.Guild.Id).Value == null)
            {
                await node.ConnectAsync(guild.Channel);
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
        private async Task ValidateLavalinkConnection(LavalinkExtension lavalinkExtension)
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
