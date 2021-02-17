using System;

namespace Curiosity.DAL
{
    /// <summary>
    /// Transaction in work with <see cref="ICuriosityDataContext"/>.
    /// </summary>
    public interface ICuriosityDataContextTransaction : IDisposable
    {
        /// <summary>
        /// Gets the transaction identifier.
        /// </summary>
        Guid TransactionId { get; }
        
        /// <summary>
        /// Name of database.
        /// </summary>
        string DbName { get; }

        /// <summary>
        /// Commits all changes made to the database in the current transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Discards all changes made to the database in the current transaction.
        /// </summary>
        void Rollback();
    }
}