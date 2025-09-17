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
        Task<T> FindByConditionAsync(Func<T, bool> expression);
        Task<IList<T>> FindAllAsync();
        Task<T?> GetByIdAsync(Object id);
        Task<T> InsertAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(Object id);
        Task SaveAsync();
        Task<IList<T>> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);
        Task<IList<T>> FilterByAsync(Expression<Func<T, bool>> predicate);


    }
}
