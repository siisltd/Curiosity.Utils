using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.DAL
{
    /// <summary>
    /// Data context with read and write access to database.
    /// </summary>
    public interface ICuriosityDataContext : ICuriosityReadOnlyDataContext
    {
        /// <summary>
        /// Notifies when transaction successfully completed.
        /// </summary>
        event Action OnTransactionCompleted;

        /// <summary>
        /// Notifies when transaction successfully completed.
        /// </summary>
        event Func<CancellationToken, Task> OnTransactionCompletedAsync;

        /// <summary>
        /// Saves changes.
        /// </summary>
        int SaveChanges();
        
        /// <summary>
        /// Saves changes in async manner.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Saves changes in async manner.
        /// </summary>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        ICuriosityDataContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        
        /// <summary>
        /// Starts a new transaction in async manner.
        /// </summary>
        Task<ICuriosityDataContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

        /// <summary>
        /// Attaches current data context to specified transaction.
        /// </summary>
        void UseTransaction(DbTransaction transaction);
    }
}
