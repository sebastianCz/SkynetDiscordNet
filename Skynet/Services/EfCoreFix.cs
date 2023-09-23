using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Skynet.db;

public class YourDbContextFactory : IDesignTimeDbContextFactory<BotContext>
{
    public BotContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
        var connectionString = configuration.GetConnectionString("DiscordBot");
        var optionsBuilder = new DbContextOptionsBuilder<BotContext>();
        optionsBuilder.UseSqlServer(connectionString);

        return new BotContext(optionsBuilder.Options);
    }
}