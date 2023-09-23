using Azure.Core.GeoJson;
using Google.Apis.YouTube.v3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Skynet.db;
using Skynet.Domain;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;

namespace Skynet.Repository.Repository
{
    public class EntityBaseRepo<T> : IEntityBaseRepo<T> where T : class,IEntityBase
    {
        internal BotContext _context;

        public EntityBaseRepo()
        {
                _context = new BotContext();
        }
        public EntityBaseRepo(BotContext context)
        {
            _context = context;
        }
        public async Task<List<T>> GetAllAsync()
        { 
            return await _context.Set<T>().ToListAsync();
        }
         
        public async Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> query = _context.Set<T>();
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            return await query.ToListAsync();
        } 


        public async Task<T> GetByIdAsync(int id)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(x => x.Id == id);
        } 
       
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }
       
        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(T entity)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        } 
        public async Task DeleteAsync<T>(T song)
        {
            _context.Remove(song);
            return;
        }

    }
    } 
