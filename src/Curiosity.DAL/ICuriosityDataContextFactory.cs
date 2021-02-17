namespace Curiosity.DAL
{
    /// <summary>
    /// Data context factory (for <see cref="ICuriosityDataContext"/>) with typed returned value.
    /// </summary>
    public interface ICuriosityDataContextFactory<out TReadWriteContext, out TReadOnlyContext> : ICuriosityDataContextFactory
        where TReadWriteContext : ICuriosityDataContext
        where TReadOnlyContext  : ICuriosityReadOnlyDataContext
    {
        /// <summary>
        /// Creates context <see cref="TReadWriteContext"/> for data access.
        /// </summary>
        /// <returns>New instance of data context</returns>
        new TReadWriteContext CreateContext();
        
        /// <summary>
        /// Creates context <see cref="TReadOnlyContext"/> for data access  from replica (if no replica, main connection will be used).
        /// </summary>
        /// <returns>New instance of data context</returns>
        new TReadOnlyContext CreateReplicaContext();
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
        ICuriosityDataContext CreateContext();
 
        /// <summary>
        /// Creates context <see cref="ICuriosityReadOnlyDataContext"/> for data access from replica (if no replica, main connection will be used).
        /// </summary>
        /// <returns>New instance of data context</returns>
        ICuriosityReadOnlyDataContext CreateReplicaContext();
    }
}