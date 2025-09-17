using Microsoft.EntityFrameworkCore;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Infrastructure.DatabaseSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Infrastructure.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext appDb)
        {
            _context = appDb;
            _dbSet = _context.Set<T>();
        }

        public IQueryable<T> Entity => _dbSet;

        public Task DeleteAsync(object id)
        {
           var entity =  _dbSet.Find(id);
            if(entity != null)
            {
                _dbSet.Remove(entity);              
            }
            return Task.CompletedTask;
        }

        public async Task<IList<T>> FilterByAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> FindByConditionAsync(Func<T, bool> expression)
        {
            return await Task.FromResult(_dbSet.FirstOrDefault(x => expression(x)));
        }

        public Task<T?> GetByIdAsync(object id)
        {
            var entity = _dbSet.FindAsync(id);
            return entity.AsTask();
        }

        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            

        }

        public Task<T> UpdateAsync(T entity)
        {
            var id = _context.Entry(entity).Property("Id").CurrentValue;
            var exist = _dbSet.Find(id);
            if (exist != null)
            {
                _context.Entry(exist).CurrentValues.SetValues(entity);
                _context.SaveChanges();
            }
            return Task.FromResult(entity);
        }
    }
}
