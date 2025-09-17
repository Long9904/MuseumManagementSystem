using MuseumSystem.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Infrastructure.Implementation
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        public IQueryable<T> Entity => throw new NotImplementedException();

        public Task DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> FilterByAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> FindAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IList<T>> FindAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, string? includeProperties = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByConditionAsync(Func<T, bool> expression)
        {
            throw new NotImplementedException();
        }

        public Task<T?> GetByIdAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<T> InsertAsync(T entity)
        {
            throw new NotImplementedException();
        }

        public Task SaveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
