using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuseumSystem.Domain.Interface;

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

        public IAreaRepository AreaRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IMuseumRepository MuseumRepository { get; }
    }
}
