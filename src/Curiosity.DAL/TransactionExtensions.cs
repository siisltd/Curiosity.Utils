using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Curiosity.DAL
{
    /// <summary>
    /// Helpers for transactions.
    /// </summary>
    public static class TransactionHelper
    {
        /// <summary>
        /// Tries to rollback all transactions.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="transactions"></param>
        public static void SafeRollbackTransactions(ILogger logger, params ICuriosityDataContextTransaction?[] transactions)
        {
            for (var i = 0; i < transactions.Length; i++)
            {
                var transaction = transactions[i];
                if (transaction == null) continue;

                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Transaction rollback failed for DB \"{transaction.DbName}\". Reason: {ex.Message}");
                }
            }
        }
        
        /// <summary>
        /// Tries to rollback all transactions.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="transactions"></param>
        public static async Task SafeRollbackTransactionsAsync(ILogger logger, CancellationToken token = default, params ICuriosityDataContextTransaction?[] transactions)
        {
            for (var i = 0; i < transactions.Length; i++)
            {
                var transaction = transactions[i];
                if (transaction == null) continue;

                try
                {
                    await transaction.RollbackAsync(token);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Transaction rollback failed for DB \"{transaction.DbName}\". Reason: {ex.Message}");
                }
            }
        }
    }
}