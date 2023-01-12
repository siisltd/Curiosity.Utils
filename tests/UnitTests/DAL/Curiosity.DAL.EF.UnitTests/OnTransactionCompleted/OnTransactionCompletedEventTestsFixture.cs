using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Curiosity.DAL.EF.UnitTests.OnTransactionCompleted
{
    /// <summary>
    /// Fixture for <see cref="OnTransactionCompletedEventTests"/>.
    /// </summary>
    public class OnTransactionCompletedEventTestsFixture
    {
        /// <summary>
        /// Creates new instance of <see cref="TransactionCompletedTestContext"/>.
        /// </summary>
        public TransactionCompletedTestContext CreateContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TransactionCompletedTestContext>();
            optionsBuilder.UseInMemoryDatabase(nameof(OnTransactionCompletedEventTestsFixture));
            optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));

            return new TransactionCompletedTestContext(optionsBuilder.Options);
        }
    }
}
