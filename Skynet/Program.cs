namespace Skynet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bot = new BotConfig();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}