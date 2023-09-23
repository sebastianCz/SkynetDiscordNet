using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Skynet.db;
using Skynet.Domain.GuildData;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace Skynet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotConfig();
            bot.RunAsync(args).GetAwaiter().GetResult();
            ////This needs to remain here, with dsharp plus online making a migration takes 10 minutes. 
            //bot.DbConfig(args).GetAwaiter().GetResult();
            //Thread.Sleep(-1);
        }

    }
    
}