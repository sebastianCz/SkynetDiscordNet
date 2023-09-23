﻿using System.Linq.Expressions;

namespace Skynet.Repository.Repository;

public interface IEntityBaseRepo <T> where T : class
{
   
    public Task<List<T>> GetAllAsync();  
    public Task<T> GetByIdAsync(int id); 
    public Task<List<T>> GetAllAsync(params Expression<Func<T, object>>[] includeProperties);  
    public Task AddAsync(T entity); 
    public Task UpdateAsync(T entity);
    public Task DeleteAsync(T entity); 

}