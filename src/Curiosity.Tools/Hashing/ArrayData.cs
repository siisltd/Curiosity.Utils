using System;

namespace Curiosity.Tools.Hashing
{
    internal sealed class ArrayData : UnifiedData
    {
        /// <inheritdoc />
        public override long Length => _data.LongLength;

        private readonly byte[] _data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArrayData"/> class.
        /// </summary>
        /// <param name="data">The data to represent.</param>
        public ArrayData(byte[] data)
        {
            _data = data;
            BufferSize = (
                _data.Length > 0 ?
                    _data.Length :
                    1);
        }

        /// <inheritdoc />
        public override void ForEachRead(Action<byte[], int, int> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(_data, 0, _data.Length);
        }

        /// <inheritdoc />
        public override void ForEachGroup(int groupSize, Action<byte[], int, int> action, Action<byte[], int, int>? remainderAction)
        {
            if (groupSize <= 0) throw new ArgumentOutOfRangeException("groupSize", "bufferSize must be greater than 0.");
            if (action == null) throw new ArgumentNullException("action");

            var remainderLength = _data.Length % groupSize;

            if (_data.Length - remainderLength > 0)
                action(_data, 0, _data.Length - remainderLength);

            if (remainderAction != null && remainderLength > 0)
                remainderAction(_data, _data.Length - remainderLength, remainderLength);
        }

        /// <inheritdoc />
        public override byte[] ToArray()
        {
            return _data;
        }
    }
}