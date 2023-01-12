using System.Collections.Generic;
using Dapper;

namespace Curiosity.DAL.Dapper
{
    internal static class DynamicParamsExtension
    {
        public static DynamicParameters ConvertToDynamicParameters(
            this IDictionary<string, object> parameters,
            bool ignoreNulls = true)
        {
            return ignoreNulls 
                ? ConvertIgnoreNulls(parameters)
                : ConvertWithNulls(parameters);
        }

        private static DynamicParameters ConvertIgnoreNulls(IDictionary<string, object>? parameters)
        {
            var dynamicParams = new DynamicParameters();
            if (parameters == null) return dynamicParams;
            
            foreach (var param in parameters)
            {
                if(param.Value != null)
                    dynamicParams.Add(param.Key, param.Value);
            }

            return dynamicParams;
        }

        private static DynamicParameters ConvertWithNulls(IDictionary<string, object>? parameters)
        {
            var dynamicParams = new DynamicParameters();
            if (parameters == null) return dynamicParams;
            
            foreach (var param in parameters)
            {
                dynamicParams.Add(param.Key, param.Value);
            }

            return dynamicParams;
        }
    }
}