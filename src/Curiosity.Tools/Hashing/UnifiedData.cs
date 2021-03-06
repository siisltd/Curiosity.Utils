using System;

namespace Curiosity.Tools.Hashing
{
    /// <summary>
    /// Centralized methodology for accessing data used by Data.HashFunction.
    /// </summary>
    public abstract class UnifiedData
    {
        /// <summary>
        /// Length of data provided.
        /// </summary>
        /// <remarks>
        /// Implementors are allowed throw an exception if it is not possible to resolve the length of the data.
        /// </remarks>
        public abstract long Length { get; }

        /// <summary>
        /// Length of temporary buffers used, if they are needed.
        /// </summary>
        /// <remarks>
        /// Implementors are not required to use this value.
        /// </remarks>
        public int BufferSize 
        {
            get => _bufferSize;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), "value must be greater than 0");

                _bufferSize = value;
            }
        }

        private int _bufferSize = 4096;

        /// <summary>
        /// Executes an action each time a chunk is read.
        /// </summary>
        /// <param name="action">Function to execute.</param>
        public abstract void ForEachRead(Action<byte[], int, int> action);

        /// <summary>
        /// Executes an action one or more times, providing the data read as an array whose length is a multiple of groupSize.  
        /// Optionally runs an action on the final remainder group.
        /// </summary>
        /// <param name="groupSize">Length of the groups passed to the action.</param>
        /// <param name="action">Action to execute for each full group read.</param>
        /// <param name="remainderAction">Action to execute if the final group is less than groupSize.  Null values are allowed.</param>
        /// <remarks>remainderAction will not be run if the length of the data is a multiple of groupSize.</remarks>
        public abstract void ForEachGroup(int groupSize, Action<byte[], int, int> action, Action<byte[], int, int> remainderAction);
        
        /// <summary>
        /// Reads all data and converts it to an in-memory array.
        /// </summary>
        /// <returns>Array of bytes read from the data provider.</returns>
        public abstract byte[] ToArray();
    }
}