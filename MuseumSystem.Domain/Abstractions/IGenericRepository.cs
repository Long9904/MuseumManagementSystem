using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Abstractions
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> Entity { get; }
        Task<T?> FindByConditionAsync(Expression<Func<T, bool>> predicate);
        Task<IList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Object id);
        Task InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Object id);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        Task<IList<T>> FilterByAsync(Expression<Func<T, bool>> predicate);
        Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize);


    }
}
