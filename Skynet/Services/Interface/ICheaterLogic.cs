using Skynet.Domain;

namespace Skynet.Services.Interface
{
    public interface ICheaterLogic
    {
        public Task<Search> IsCheater(string steamProfileLink);
        public Task<Search> AddCheater(string steamProfileLink, string updaterName);
        public Task<Search> DeleteCheater(string steamProfileLink, string updaterName);
        public Task<Search> DisplayAll();
    }
}
