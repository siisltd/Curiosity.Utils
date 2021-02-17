using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.DAL
{   
    /// <summary>
    /// Data context with read only access.
    /// </summary>
    public interface ICuriosityReadOnlyDataContext : IDisposable
    {
        /// <summary>
        /// Timeout of command execution.
        /// </summary>
        int? CommandTimeoutSec { get; set; }                     

        /// <summary>
        /// Returns current connection to database.
        /// </summary>
        IDbConnection Connection { get; }
        
        /// <summary>
        /// Returns current date and time from database in UTC.
        /// </summary>
        Task<DateTime> GetImmediateServerTimeUtcAsync(CancellationToken cancellationToken = default);
    }
}