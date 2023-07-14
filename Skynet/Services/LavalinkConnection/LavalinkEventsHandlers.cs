namespace Skynet.Services.LavalinkConnection
{
    public class LavalinkEventsHandlers
    {
        public async Task PlaybackStarted(object sender, EventArgs e)
        {
            Console.WriteLine($"PlaybackStarted");
        }
        public async Task PlaybackEnded(object sender, EventArgs e)
        {

            Console.WriteLine($"PlaybackEnded");
        }
    }
}
