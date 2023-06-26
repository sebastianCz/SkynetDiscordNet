namespace Skynet.Services.Interface
{
    public interface IValidation
    {
        public Task<string> ValidateSteamProfileLink(string profileLink);
    }
}
