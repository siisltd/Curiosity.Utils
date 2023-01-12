using System.Threading;

namespace Curiosity.Tools
{
    /// <summary>
    /// Thread safe sequence counter that is used when converting
    /// </summary>
    public class SequenceCounter
    {
        private int _sequenceNumber;

        /// <summary>
        /// Returns next value for a sequence.
        /// </summary>
        public int GetNext() => Interlocked.Increment(ref _sequenceNumber);

        /// <summary>
        /// Resets counter.
        /// </summary>
        public void Reset() => Interlocked.Exchange(ref _sequenceNumber, 0);
    }
}
