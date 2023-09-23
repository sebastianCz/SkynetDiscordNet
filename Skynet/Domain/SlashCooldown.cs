using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using System.Collections.Concurrent;

public class SlashCooldownAttribute : SlashCheckBaseAttribute
{
    private int _usesPerGuild { get; set; }
    private int _cooldown { get; set; }
    private static readonly ConcurrentDictionary<ulong, DateTime> GuildCooldowns = new ConcurrentDictionary<ulong, DateTime>();
    public SlashCooldownAttribute(int usesPerGuild, int cooldown)
    {
        _usesPerGuild = usesPerGuild;
        _cooldown = cooldown;
    }

    public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
    {
        var cooldownStatus = await IsOnCooldown(ctx);
        if (cooldownStatus.Item1)
        {
            await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
            .WithContent(cooldownStatus.Item2)
            );
            return false;
        }
        else
        {
            await ctx.CreateResponseAsync(
                           InteractionResponseType.ChannelMessageWithSource,
                           new DiscordInteractionResponseBuilder()
                           .WithContent(cooldownStatus.Item2)
                           );
            return true;
        }
    }
    private async Task<(bool, string)> IsOnCooldown(InteractionContext ctx)
    {
        var guildCooldown = GuildCooldowns.TryGetValue(ctx.Member.Guild.Id, out var lastTimeUsed);
        var remainingCooldown = _cooldown - (DateTime.UtcNow - lastTimeUsed).Seconds;
        // Check if the user is on cooldown
        if (guildCooldown && (DateTime.UtcNow - lastTimeUsed).Seconds < _cooldown)
        {
            return (true, $"Command is on cooldown. Try again in {remainingCooldown} seconds");
        }

        if (!guildCooldown) { GuildCooldowns.TryAdd(ctx.Member.Guild.Id, DateTime.UtcNow); }
        else { GuildCooldowns[ctx.Member.Guild.Id] = DateTime.UtcNow; }
        return (false, $"Command received. ");
    }
}