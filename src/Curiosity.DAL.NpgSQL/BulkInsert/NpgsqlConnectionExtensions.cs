using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;

namespace Curiosity.DAL.NpgSQL.BulkInsert
{
    /// <summary>
    /// Extensions methods for <see cref="NpgsqlConnection"/>.
    /// </summary>
    public static class NpgsqlConnectionExtensions
    {
        /// <summary>
        /// Executes bulk insert.
        /// </summary>
        /// <param name="connection">Connection with Postgres</param>
        /// <param name="copyCommand">SQL with copy command</param>
        /// <param name="data">Data for bulk insert</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public static async Task BulkInsertViaCopyAsync<T>(
            this NpgsqlConnection connection, 
            string copyCommand, 
            IEnumerable<T> data,
            CancellationToken cancellationToken = default) where T: IBulkInsertable
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (String.IsNullOrEmpty(copyCommand)) throw new ArgumentNullException(nameof(copyCommand));
            if (data == null) throw new ArgumentNullException(nameof(data));

            using (var writer = connection.BeginBinaryImport(copyCommand))
            {
                //todo #1 maybe use ICollection and usual for loop
                foreach (var item in data)
                {
                    if (cancellationToken.IsCancellationRequested)
                        throw new OperationCanceledException();
                    
                    item.WriteToStream(writer);
                }
                
                // todo #2 maybe use continuations?
                await writer.CompleteAsync(cancellationToken);
            }
        }
    }
}