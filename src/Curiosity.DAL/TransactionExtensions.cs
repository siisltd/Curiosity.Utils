using System;
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
            foreach (var transaction in transactions)
            {
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
    }
}