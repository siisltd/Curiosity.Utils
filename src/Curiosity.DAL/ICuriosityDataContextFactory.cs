namespace Curiosity.DAL
{
    /// <summary>
    /// Data context factory (for <see cref="ICuriosityDataContext"/>) with typed returned value.
    /// </summary>
    public interface ICuriosityDataContextFactory<out TContext> : ICuriosityDataContextFactory
        where TContext : ICuriosityDataContext
    {
        /// <summary>
        /// Creates context <see cref="TContext"/> for data access.
        /// </summary>
        /// <returns>New instance of data context</returns>
        new TContext CreateContext(bool isLoggingEnabled = false);
    }
    
    /// <summary>
    /// Data context factory (for <see cref="ICuriosityDataContext"/>) with typed returned value.
    /// </summary>
    public interface ICuriosityReadOnlyDataContextFactory<out TContext> : ICuriosityReadOnlyDataContextFactory
        where TContext  : ICuriosityReadOnlyDataContext
    {
        /// <summary>
        /// Creates context <see cref="TContext"/> for read only data access (if no connection string found for read only access, main connection will be used).
        /// </summary>
        /// <returns>New instance of data context</returns>
        new TContext CreateReadOnlyContext(bool isLoggingEnabled = false);
    }

    /// <summary>
    /// Data context factory (for <see cref="ICuriosityDataContext"/>).
    /// </summary>
    public interface ICuriosityDataContextFactory
    {
        /// <summary>
        /// Creates context <see cref="ICuriosityDataContext"/> for data access.
        /// </summary>
        /// <returns>New instance of data context</returns>
        ICuriosityDataContext CreateContext(bool isLoggingEnabled = false);
    }
    
    /// <summary>
    /// Data context factory (for <see cref="ICuriosityReadOnlyDataContext"/>).
    /// </summary>
    public interface ICuriosityReadOnlyDataContextFactory
    {
        /// <summary>
        /// Creates context <see cref="ICuriosityReadOnlyDataContext"/> for read only data access (if no connection string found for read only access, main connection will be used).
        /// </summary>
        /// <returns>New instance of data context</returns>
        ICuriosityReadOnlyDataContext CreateReadOnlyContext(bool isLoggingEnabled = false);
    }
}