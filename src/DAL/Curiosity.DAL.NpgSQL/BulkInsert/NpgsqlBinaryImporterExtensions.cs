using Npgsql;
using NpgsqlTypes;

namespace Curiosity.DAL.NpgSQL.BulkInsert
{
    /// <summary>
    /// Extensions for <see cref="NpgsqlBinaryImporter"/>.
    /// </summary>
    public static class NpgsqlBinaryImporterExtensions
    {
        /// <summary>
        /// Writes nullable structure in bulk manner.
        /// </summary>
        public static void WriteNullable<T>(this NpgsqlBinaryImporter writer, T? value, NpgsqlDbType dbType) where T: struct
        {
            if (value.HasValue)
            {
                writer.Write(value.Value, dbType);
            }
            else
            {
                writer.WriteNull();
            }
        }
        
        /// <summary>
        /// Writes text value in bulk manner.
        /// </summary>
        public static void WriteNullable(this NpgsqlBinaryImporter writer, string? value, NpgsqlDbType dbType)
        {
            if (value != null)
            {
                writer.Write(value, dbType);
            }
            else
            {
                writer.WriteNull();
            }
        }
    }
}