using Domain.Entities;

namespace Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Contact> Contacts { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
