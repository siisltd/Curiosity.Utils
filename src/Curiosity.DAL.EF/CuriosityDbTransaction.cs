using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Curiosity.DAL.EF
{
    /// <inheritdoc />
    public class CuriosityDbTransaction : ICuriosityDataContextTransaction
    {
        private readonly IDbContextTransaction _efDbContextTransaction;

        private bool _wasDisposed;

        public CuriosityDbTransaction(IDbContextTransaction efDbContextTransaction)
        {
            _efDbContextTransaction = efDbContextTransaction ?? throw new ArgumentNullException(nameof(efDbContextTransaction));
        }

        /// <inheritdoc />
        public event Action? OnTransactionCompleted;

        /// <summary>
        /// Notifies when transaction successfully completed.
        /// </summary>
        public event Func<CancellationToken, Task>? OnTransactionCompletedAsync;

        /// <inheritdoc />
        public Guid TransactionId => _efDbContextTransaction.TransactionId;

        /// <inheritdoc />
        public string DbName => _efDbContextTransaction.GetDbTransaction()?.Connection?.Database!;

        /// <inheritdoc />
        public void Commit()
        {
            _efDbContextTransaction.Commit();

            OnTransactionCompleted?.Invoke();

            // this is bad but we don't have any valid ways
            if (OnTransactionCompletedAsync != null)
            {
                OnTransactionCompletedAsync.Invoke(CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        /// <inheritdoc />
        public async Task CommitAsync(CancellationToken cancellationToken = default)
        {
            await _efDbContextTransaction.CommitAsync(cancellationToken);

            OnTransactionCompleted?.Invoke();


            if (OnTransactionCompletedAsync != null)
            {
                await OnTransactionCompletedAsync.Invoke(cancellationToken);
            }
        }

        /// <inheritdoc />
        public void Rollback()
        {
            _efDbContextTransaction.Rollback();
        }
        
        /// <inheritdoc />
        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _efDbContextTransaction.RollbackAsync(cancellationToken);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_wasDisposed) return;

            _wasDisposed = true;
            _efDbContextTransaction.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            if (_wasDisposed) return new ValueTask();

            _wasDisposed = true;
            return _efDbContextTransaction.DisposeAsync();
        }
    }
}
