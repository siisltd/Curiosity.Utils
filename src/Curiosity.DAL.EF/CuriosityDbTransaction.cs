using System;
using Microsoft.EntityFrameworkCore.Storage;

namespace Curiosity.DAL.EF
{
    /// <inheritdoc />
    public class CuriosityDbTransaction : ICuriosityDataContextTransaction
    {
        private readonly IDbContextTransaction _efDbContextTransaction;

        public CuriosityDbTransaction(IDbContextTransaction efDbContextTransaction)
        {
            _efDbContextTransaction = efDbContextTransaction ?? throw new ArgumentNullException(nameof(efDbContextTransaction));
        }

        /// <inheritdoc />
        public Guid TransactionId => _efDbContextTransaction.TransactionId;

        /// <inheritdoc />
        public string DbName => _efDbContextTransaction.GetDbTransaction()?.Connection?.Database!;

        /// <inheritdoc />
        public void Commit()
        {
            _efDbContextTransaction.Commit();
        }

        /// <inheritdoc />
        public void Rollback()
        {
            _efDbContextTransaction.Rollback();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _efDbContextTransaction.Dispose();
        }
    }
}