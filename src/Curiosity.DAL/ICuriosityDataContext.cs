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
        /// Saves changes.
        /// </summary>
        int SaveChanges();
        
        /// <summary>
        /// Saves changes in async manner.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Saves changes in async manner.
        /// </summary>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);

        /// <summary>
        /// Starts a new transaction.
        /// </summary>
        ICuriosityDataContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        
        /// <summary>
        /// Starts a new transaction in async manner.
        /// </summary>
        Task<ICuriosityDataContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Attaches current data context to specified transaction.
        /// </summary>
        void UseTransaction(DbTransaction transaction);
    }
}