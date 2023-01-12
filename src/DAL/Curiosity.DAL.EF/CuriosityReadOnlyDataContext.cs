using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Curiosity.DAL.EF
{
    /// <summary>
    /// Base read-only data context.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CuriosityReadOnlyDataContext<T> :
        DbContext,
        ICuriosityReadOnlyDataContext  
        where T: DbContext
    {
        /// <inheritdoc />
        public int? CommandTimeoutSec
        {
            get => base.Database.GetCommandTimeout();
            set => base.Database.SetCommandTimeout(value);
        }

        /// <inheritdoc />
        public IDbConnection Connection => base.Database.GetDbConnection();

        protected CuriosityReadOnlyDataContext(DbContextOptions<T> options) : base(options)
        {
        }

        /// <inheritdoc />
        public virtual async Task<DateTime> GetImmediateServerTimeUtcAsync(CancellationToken cancellationToken = default)
        {
            using (var command = Connection.CreateCommand())
            {
                command.CommandText = "select timezone('UTC'::text, now());";
                
                await Database.OpenConnectionAsync(cancellationToken);
                
                var commandResult = command.ExecuteScalar();
                if (commandResult is DateTime serverTime)
                {
                    return DateTime.SpecifyKind(serverTime, DateTimeKind.Utc);
                }
                
                throw new Exception("Incorrect result from database while fetching date time");
            }
        }

        /// <summary>
        /// Specifies DateTimeKind for all date fields in models.
        /// </summary>
        /// <param name="modelBuilder">EF model builder</param>
        /// <param name="kind">Kind to specify</param>
        protected void SpecifyDateKindForAllModels(ModelBuilder modelBuilder, DateTimeKind kind = DateTimeKind.Utc)
        {
            var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
                v => v, v => DateTime.SpecifyKind(v, kind));

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime) || property.ClrType == typeof(DateTime?))
                        property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }
}
