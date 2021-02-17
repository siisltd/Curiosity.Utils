using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Curiosity.DAL.Dapper
{
    /// <inheritdoc />
    public class DapperSqlExecutor : ISqlExecutor
    {
        /// <inheritdoc />
        public void Execute(
            ICuriosityDataContext context,
            string sqlTemplate,
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false)
        {
            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            var connection = context.Connection;
            connection.Execute(sqlTemplate, dynamicParams);
        }

        /// <inheritdoc />
        public Task ExecuteAsync(
            ICuriosityDataContext context,
            string sqlTemplate,
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false)
        {
            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            var connection = context.Connection;
            return connection.ExecuteAsync(sqlTemplate, dynamicParams);
        }

        /// <inheritdoc />
        public T QuerySingleOrDefault<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate,
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false)
        {
            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            var connection = context.Connection;
            return connection.QuerySingleOrDefault<T>(sqlTemplate, dynamicParams);
        }

        /// <inheritdoc />
        public Task<T> QuerySingleOrDefaultAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate,
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false)
        {
            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            var connection = context.Connection;
            return connection.QuerySingleOrDefaultAsync<T>(sqlTemplate, dynamicParams);
        }

        /// <inheritdoc />
        public IEnumerable<T> QueryMany<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate,
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = false)
        {
            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            var connection = context.Connection;
            var result = connection.Query<T>(sqlTemplate, dynamicParams);

            return result;
        }

        /// <inheritdoc />
        public Task<IEnumerable<T>> QueryManyAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate,
            IDictionary<string, object>? parameters = null,
            bool ignoreNulls = true)
        {
            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            var connection = context.Connection;
            return connection.QueryAsync<T>(sqlTemplate, dynamicParams);
        }
        
        /// <inheritdoc />
        public Task<IEnumerable<T>> QueryManyAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string sqlTemplate,
            object? parameters)
        {
            var connection = context.Connection;
            return connection.QueryAsync<T>(sqlTemplate, parameters);
        }

        /// <inheritdoc />
        public T ExecuteStoredProcedure<T>(
            ICuriosityDataContext context,
            string procedureName,
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true)
        {
            var connection = context.Connection;

            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            return connection.QuerySingleOrDefault<T>(
                procedureName,
                dynamicParams,
                commandTimeout: timeoutSec,
                commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public Task<T> ExecuteStoredProcedureAsync<T>(
            ICuriosityDataContext context,
            string procedureName,
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true)
        {
            var connection = context.Connection;

            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            return connection.QuerySingleOrDefaultAsync<T>(
                procedureName,
                dynamicParams,
                commandTimeout: timeoutSec,
                commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public void ExecuteStoredProcedure(
            ICuriosityDataContext context,
            string procedureName,
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true)
        {
            var connection = context.Connection;

            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            connection.QuerySingleOrDefault(
                procedureName,
                dynamicParams,
                commandTimeout: timeoutSec,
                commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public Task ExecuteStoredProcedureAsync(
            ICuriosityDataContext context,
            string procedureName,
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true)
        {
            var connection = context.Connection;

            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            return connection.QuerySingleOrDefaultAsync(
                procedureName,
                dynamicParams,
                commandTimeout: timeoutSec,
                commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public IEnumerable<T> ExecuteStoredProcedure<T>(
            ICuriosityReadOnlyDataContext context,
            string procedureName,
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            var connection = context.Connection;

            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            return connection.Query<T>(
                procedureName,
                dynamicParams,
                commandTimeout: timeoutSec,
                commandType: CommandType.StoredProcedure);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(
            ICuriosityReadOnlyDataContext context,
            string procedureName,
            IDictionary<string, object>? parameters = null,
            int? timeoutSec = null,
            bool ignoreNulls = true)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var connection = context.Connection;

            var dynamicParams = parameters?.ConvertToDynamicParameters(ignoreNulls);

            var result = await connection.QueryAsync<T>(
                procedureName,
                dynamicParams,
                commandTimeout: timeoutSec,
                commandType: CommandType.StoredProcedure);

            return result;
        }
    }
}