using Microsoft.EntityFrameworkCore;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Infrastructure.DatabaseSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public async Task<IList<T>> FilterByAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim());
                }
            }

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public Task<T?> GetByIdAsync(object id)
        {
            var entity = _dbSet.FindAsync(id);
            return entity.AsTask();
        }

        public async Task InsertAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            // Warning: this method does not call SaveChanges or SaveChangesAsync.
            // Caller (UnitOfWork) call SaveChangesAsync after all operations complete.
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var id = _context.Entry(entity).Property("Id").CurrentValue;
            var exist = await _dbSet.FindAsync(id);

            if (exist == null)
            {
                throw new KeyNotFoundException($"Entity with Id {id} not found.");
            }
            _context.Entry(exist).CurrentValues.SetValues(entity);
            return exist;
            // Warning: this method does not call SaveChanges or SaveChangesAsync.
            // Caller (UnitOfWork) call SaveChangesAsync after all operations complete.
        }
    }
}
