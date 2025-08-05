using System.Data.Common;

namespace WarehouseManagement.Application;

public interface IUnitOfWork
{
    Task<DbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    DbTransaction? DbTransaction { get; }

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
