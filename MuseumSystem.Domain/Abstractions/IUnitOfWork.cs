using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;        
        Task SaveChangeAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollBack();
        bool HasActiveTransaction();
    }
}
