using System;
using Npgsql;

namespace Curiosity.DAL.NpgSQL
{
    /// <summary>
    /// Extension methods for <see cref="NpgsqlDataReader"/>.
    /// </summary>
    public static class PgSqlDataReaderExtensions
    {
        public static int? GetInt32(this NpgsqlDataReader rd, string name, int? defaultValue = null)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetInt32(n) : defaultValue;
        }

        public static int? GetInt32(this NpgsqlDataReader rd, int n, int? defaultValue = null)
        {
            return !rd.IsDBNull(n) ? rd.GetInt32(n) : defaultValue;
        }

        public static long? GetInt64(this NpgsqlDataReader rd, string name, long? defaultValue = null)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetInt64(n) : defaultValue;
        }

        public static long? GetInt64(this NpgsqlDataReader rd, int n, long? defaultValue = null)
        {
            return !rd.IsDBNull(n) ? rd.GetInt64(n) : defaultValue;
        }

        public static double? GetDouble(this NpgsqlDataReader rd, string name, double? defaultValue = null)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetDouble(n) : defaultValue;
        }

        public static double? GetDouble(this NpgsqlDataReader rd, int n, double? defaultValue = null)
        {
            return !rd.IsDBNull(n) ? rd.GetDouble(n) : defaultValue;
        }

        public static decimal? GetDecimal(this NpgsqlDataReader rd, string name, decimal? defaultValue = null)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetDecimal(n) : defaultValue;
        }

        public static decimal? GetDecimal(this NpgsqlDataReader rd, int n, decimal? defaultValue = null)
        {
            return !rd.IsDBNull(n) ? rd.GetDecimal(n) : defaultValue;
        }

        public static DateTime? GetDateTime(this NpgsqlDataReader rd, string name, DateTime? defaultValue)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetDateTime(n) : defaultValue;
        }

        public static DateTime? GetDateTime(this NpgsqlDataReader rd, int n, DateTime? defaultValue = null)
        {
            return !rd.IsDBNull(n) ? rd.GetDateTime(n) : defaultValue;
        }

        public static Guid? GetGuid(this NpgsqlDataReader rd, string name, Guid? defaultValue = null)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetGuid(n) : defaultValue;
        }

        public static Guid? GetGuid(this NpgsqlDataReader rd, int n, Guid? defaultValue = null)
        {
            return !rd.IsDBNull(n) ? rd.GetGuid(n) : defaultValue;
        }

        public static string? GetString(this NpgsqlDataReader rd, string name)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetString(n) : null;
        }

        public static string GetString(this NpgsqlDataReader rd, string name, string defaultValue)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n) ? rd.GetString(n) : defaultValue;
        }

        public static string GetString(this NpgsqlDataReader rd, int ordinal, string defaultValue)
        {
            return !rd.IsDBNull(ordinal) ? rd.GetString(ordinal) : defaultValue;
        }

        public static DateTime GetDateTime(this NpgsqlDataReader rd, string name)
        {
            var n = rd.GetOrdinal(name);
            return !rd.IsDBNull(n)
                ? rd.GetDateTime(n)
                : throw new ArgumentException($"Ordinal with name {name} should not have null value");
        }

        public static bool IsDBNull(this NpgsqlDataReader rd, string name)
        {
            var n = rd.GetOrdinal(name);
            return rd.IsDBNull(n);
        }
    }
}