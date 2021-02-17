using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.AppInitializer
{
    /// <summary>
    /// Represents a type that performs async application initialization.
    /// </summary>
    public interface IAppInitializer
    {
        /// <summary>
        /// Performs async initialization.
        /// </summary>
        /// <returns>A task that represents the initialization completion.</returns>
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}