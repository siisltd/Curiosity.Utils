using System;

namespace Curiosity.RequestProcessing.Postgres
{
    /// <summary>
    /// Информация о БД, события от которой мы слушаем.
    /// </summary>
    public class MonitoredDatabase : IEventSource<MonitoredDatabase>
    {
        /// <summary>
        /// Строка подключения к БД.
        /// </summary>
        public string ConnectionString { get; }

        /// <inheritdoc cref="MonitoredDatabase"/>
        public MonitoredDatabase(string connectionString)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            ConnectionString = connectionString;
        }

        /// <inheritdoc />
        public bool Equals(MonitoredDatabase? other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ConnectionString == other.ConnectionString;
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MonitoredDatabase) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return ConnectionString.GetHashCode();
        }

        /// <summary>
        /// Equal compare.
        /// </summary>
        public static bool operator ==(MonitoredDatabase? left, MonitoredDatabase? right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Not equal compare.
        /// </summary>
        public static bool operator !=(MonitoredDatabase? left, MonitoredDatabase? right)
        {
            return !Equals(left, right);
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"\"{ConnectionString}\"";
        }
    }
}
