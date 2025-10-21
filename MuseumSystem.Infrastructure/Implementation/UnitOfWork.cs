using Microsoft.EntityFrameworkCore.Storage;
using MuseumSystem.Domain.Abstractions;
using MuseumSystem.Domain.Interface;
using MuseumSystem.Infrastructure.DatabaseSetting;
using MuseumSystem.Infrastructure.Repositories;


namespace MuseumSystem.Infrastructure.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private bool disposed = false;
        private IDbContextTransaction? _transaction;

        private Dictionary<Type, object> repositories;

        public IAreaRepository AreaRepository { get; private set; }

        public IAccountRepository AccountRepository { get; private set; }

        public IMuseumRepository MuseumRepository { get; private set; }

        public IDisplayPositionRepository DisplayPositionRepository { get; private set; }

        public IArtifactRepository ArtifactRepository { get; private set; }

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            repositories = new Dictionary<Type, object>();
            AreaRepository = new AreaRepository(_context);
            AccountRepository = new AccountRepository(_context);
            MuseumRepository = new MuseumRepository(_context);
            DisplayPositionRepository = new DisplayPositionRepository(_context);
            ArtifactRepository = new ArtifactRepository(_context);
        }


        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
                _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                if (_transaction != null)
                    await _transaction.CommitAsync();
            }
            catch
            {
                await RollBackAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IGenericRepository<T> GetRepository<T>() where T : class
        {
            var type = typeof(T);
            if (!repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<T>(_context);
                repositories.Add(type, repositoryInstance);
            }
            return (IGenericRepository<T>)repositories[type];
        }

        public bool HasActiveTransaction()
        {
            throw new NotImplementedException();
        }

        public async Task RollBackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
