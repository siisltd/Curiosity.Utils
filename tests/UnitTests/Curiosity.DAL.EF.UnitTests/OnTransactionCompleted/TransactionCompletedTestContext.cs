using Microsoft.EntityFrameworkCore;

namespace Curiosity.DAL.EF.UnitTests.OnTransactionCompleted
{
    /// <summary>
    /// Data context for <see cref="OnTransactionCompletedEventTests"/>.
    /// </summary>
    public class TransactionCompletedTestContext : CuriosityDataContext<TransactionCompletedTestContext>
    {
        public TransactionCompletedTestContext(DbContextOptions<TransactionCompletedTestContext> options) : base(options)
        {
        }
    }
}
