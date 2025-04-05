using Microsoft.EntityFrameworkCore.Storage;

namespace ITC.Repositories.Interface
{
    public interface IUnitOfWork : IDisposable
    { 
        IGenericRepository<T> GetRepository<T>() where T : class;
        void Save();
        Task SaveAsync();
        void BeginTransaction();
        void CommitTransaction();
        void RollBack();

		Task<IDbContextTransaction> BeginTransactionAsync();
	}
}
