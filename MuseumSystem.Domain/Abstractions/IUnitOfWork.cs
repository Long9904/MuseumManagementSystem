using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Domain.Abstractions
{
    public interface IUnitOfWork
    {
        IGenericRepository<T> GetRepository<T>() where T : class;
        void Save();
        Task SaveChangeAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollBack();
        bool HasActiveTransaction();
    }
}
