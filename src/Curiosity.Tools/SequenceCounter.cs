namespace Curiosity.Tools
{
    /// <summary>
    /// Sequence counter that is used when converting
    /// </summary>
    public class SequenceCounter
    {
        private int _sequenceNumber;

        /// <summary>
        /// Returns next value for a sequence.
        /// </summary>
        public int GetNext() => _sequenceNumber++;

        /// <summary>
        /// Resets counter.
        /// </summary>
        public void Reset() => _sequenceNumber = 0;
    }
}