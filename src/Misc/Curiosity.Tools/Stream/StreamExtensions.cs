using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.Tools
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        public static void CopyTo(this Stream source, Stream destination, byte[] buffer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
                throw new ArgumentException("Buffer is too small", nameof(buffer));

            var copying = true;

            while (copying)
            {
                var bytesRead = source.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                }
                else
                {
                    destination.Flush();
                    copying = false;
                }
            }
        }

        /// <summary>
        /// Copy the contents of one <see cref="Stream"/> to another.
        /// </summary>
        /// <param name="source">The stream to source data from.</param>
        /// <param name="destination">The stream to write data to.</param>
        /// <param name="buffer">The buffer to use during copying.</param>
        /// <param name="cts">Cancellation token</param>
        public static async Task CopyToAsync(this Stream source, Stream destination, byte[] buffer, CancellationToken cts = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (destination == null)
                throw new ArgumentNullException(nameof(destination));

            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            // Ensure a reasonable size of buffer is used without being prohibitive.
            if (buffer.Length < 128)
                throw new ArgumentException("Buffer is too small", nameof(buffer));

            var copying = true;

            while (copying)
            {
                var bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, cts);
                if (bytesRead > 0)
                {
                    await destination.WriteAsync(buffer, 0, bytesRead, cts);
                }
                else
                {
                    await destination.FlushAsync(cts);
                    copying = false;
                }
            }
        }
    }
}