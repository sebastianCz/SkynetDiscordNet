using DSharpPlus.SlashCommands;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Skynet.Domain.GuildData;
using Skynet.Repository.Repository;
using System.Linq.Expressions;

namespace Skynet.db.Repo
{
    public class MusicUsersRepo : EntityBaseRepo<MusicUser>, IMusicUserRepo<MusicUser>
    {
        private BotContext _context;
        public MusicUsersRepo(BotContext context) : base(context)
        {
            _context = context;
        } 
        public async Task<MusicUser> EnsureCreated(string userName)
        {
            var musicUser = await this.GetByNameAsync(userName);
            if (musicUser == null)
            {
                musicUser = new MusicUser()
                {
                    UserName = userName,
                    LastUsed = DateTime.Now, 
                    
                };
                await this.AddAsync(musicUser);
            }
            return musicUser;
        }
        public async Task<MusicUser> GetByNameAsync(string id, params Expression<Func<MusicUser, object>>[] includeProperties)
        {
            IQueryable<MusicUser> query = _context.Set<MusicUser>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query
                .Include(x=>x.SearchTerms)
                .SingleOrDefaultAsync(x=>x.UserName == id);
        }

        public Task<MusicUser> GetByDiscordIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<MusicUser> GetUserForTermAddition(string name)
        {
            var x = new InteractionContext(); 
            var result = await _context.MusicUsers
                .Include(x=>x.SearchTerms)
                .SingleOrDefaultAsync(x=>x.UserName == name);  
            return result;
        }
       
        }
}
