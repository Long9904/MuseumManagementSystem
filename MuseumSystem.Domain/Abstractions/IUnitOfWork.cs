using System;
using System.Threading.Tasks;
using MuseumSystem.Domain.Interface;

namespace MuseumSystem.Domain.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;

        Task SaveChangeAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollBackAsync();
        bool HasActiveTransaction();

        public IAreaRepository AreaRepository { get; }
        public IAccountRepository AccountRepository { get; }
        public IMuseumRepository MuseumRepository { get; }
        public IDisplayPositionRepository DisplayPositionRepository { get; }
        public IArtifactRepository ArtifactRepository { get; }

    }
}
