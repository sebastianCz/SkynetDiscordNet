using Skynet.db;
using Skynet.db.Repo;
using Skynet.Domain.GuildData;

namespace ContosoUniversity.DAL
{
    public class UnitOfWork : IDisposable
    {
        private BotContext _context = new BotContext();
        private GuildMusicDataRepo _guildMusicData;
        private MusicUsersRepo _musicUser;
        private bool disposed = false;
        internal BotContext Context
        {
            get
            {

                if (this._context == null || disposed)
                {
                    this._context = new BotContext();
                }
                return _context;
            }
        }
        public MusicUsersRepo MusicUser
        {
            get
            {

                if (this._musicUser == null)
                {
                    this._musicUser = new MusicUsersRepo(_context);
                }
                return _musicUser;
            }
        }
        public GuildMusicDataRepo GuildMusicData
        {
            get
            {

                if (this._guildMusicData == null)
                {
                    this._guildMusicData = new GuildMusicDataRepo(_context);
                }
                return _guildMusicData;
            }
        }
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

       

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}