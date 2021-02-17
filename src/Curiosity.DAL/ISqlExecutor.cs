using System.Collections.Generic;
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
        void Execute(
            ICuriosityDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false);

        /// <summary>
        /// Executes SQl query.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        Task ExecuteAsync(
            ICuriosityDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false);

        /// <summary>
        /// Executes SQl query and returns value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        T QuerySingleOrDefault<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false);

        /// <summary>
        /// Executes SQl query and returns value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        Task<T> QuerySingleOrDefaultAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false);
        
        /// <summary>
        /// Executes SQl query and returns collection of values.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        IEnumerable<T> QueryMany<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false);

        /// <summary>
        /// Executes SQl query and returns collection of value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        Task<IEnumerable<T>> QueryManyAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = true);
        
        /// <summary>
        /// Executes SQl query and returns collection of value.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="sqlTemplate">SQL query (can contains parameters placeholders)</param>
        /// <param name="parameters">Parameters for query</param>
        Task<IEnumerable<T>> QueryManyAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate, 
            object? parameters = null);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="timeoutSec">Timeout for stored procedure</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        T ExecuteStoredProcedure<T>(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="timeoutSec">Timeout for stored procedure</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        Task<T> ExecuteStoredProcedureAsync<T>(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="timeoutSec">Timeout for stored procedure</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        void ExecuteStoredProcedure(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="timeoutSec">Timeout for stored procedure</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        Task ExecuteStoredProcedureAsync(
            ICuriosityDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="timeoutSec">Timeout for stored procedure</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        IEnumerable<T> ExecuteStoredProcedure<T>(
            ICuriosityReadOnlyDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true);

        /// <summary>
        /// Executes stored procedure and returns a result.
        /// </summary>
        /// <param name="context">Data context for query execution</param>
        /// <param name="procedureName">Stored procedure name</param>
        /// <param name="parameters">Parameters for stored procedure</param>
        /// <param name="timeoutSec">Timeout for stored procedure</param>
        /// <param name="ignoreNulls">Null params will be ignored</param>
        Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string procedureName, 
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true);
    }
}