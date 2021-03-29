using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.DAL
{
    /// <summary>
    /// Executor of SQL queries.
    /// </summary>
    public interface ISqlExecutor
    {
        /// <summary>
        /// Executes SQl query.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        void Execute(
            ICuriosityDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false,
            int? commandTimeoutSec = null);

        /// <summary>
        /// Executes SQl query.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ExecuteAsync(
            ICuriosityDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false,
            int? commandTimeoutSec = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes SQl query and returns value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        T QuerySingleOrDefault<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false,
            int? commandTimeoutSec = null);

        /// <summary>
        /// Executes SQl query and returns value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<T> QuerySingleOrDefaultAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false,
            int? commandTimeoutSec = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Executes SQl query and returns collection of values.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        IEnumerable<T> QueryMany<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false,
            int? commandTimeoutSec = null);

        /// <summary>
        /// Executes SQl query and returns collection of value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IEnumerable<T>> QueryManyAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = true,
            int? commandTimeoutSec = null,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Executes SQl query and returns collection of value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IEnumerable<T>> QueryManyAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            object? parameters = null,
            int? commandTimeoutSec = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        T ExecuteStoredProcedure<T>(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? commandTimeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<T> ExecuteStoredProcedureAsync<T>(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? commandTimeoutSec = null,
            bool ignoreNulls = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes stored procedure.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        void ExecuteStoredProcedure(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? commandTimeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task ExecuteStoredProcedureAsync(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? commandTimeoutSec = null,
            bool ignoreNulls = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        IEnumerable<T> ExecuteStoredProcedure<T>(
            ICuriosityReadOnlyDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? commandTimeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="commandTimeoutSec">Number of seconds before command execution timeout.</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? commandTimeoutSec = null,
            bool ignoreNulls = true,
            CancellationToken cancellationToken = default);
    }
}