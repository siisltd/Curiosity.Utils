using Npgsql;

namespace Curiosity.DAL.NpgSQL.BulkInsert
{
    /// <summary>
    /// Entity that supports bulk insert via copy command.
    /// </summary>
    public interface IBulkInsertable
    {
        /// <summary>
        /// Writes entity to the stream.
        /// </summary>
        void WriteToStream(NpgsqlBinaryImporter writer);
    }
}