using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Curiosity.DAL.EF
{
    /// <summary>
    /// Base read-write data context at Curiosity projects
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CuriosityDataContext<T> : CuriosityReadOnlyDataContext<T>, ICuriosityDataContext  where T: DbContext
    {
        /// <inheritdoc />
        public CuriosityDataContext(DbContextOptions<T> options) : base(options)
        {
        }

        /// <inheritdoc />
        public ICuriosityDataContextTransaction BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transaction = base.Database.BeginTransaction(isolationLevel);
            
            return new CuriosityDbTransaction(transaction);
        }

        /// <inheritdoc />
        public async Task<ICuriosityDataContextTransaction> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            var transaction = await base.Database.BeginTransactionAsync(isolationLevel);
            
            return new CuriosityDbTransaction(transaction);
        }

        /// <inheritdoc />
        public void UseTransaction(DbTransaction transaction)
        {
            Database.UseTransaction(transaction);
        }
    }
}