using System;
using System.Threading;

namespace Curiosity.Tools
{

    public class UniqueIdGenerationException : Exception
    {
        public UniqueIdGenerationException(string message) : base(message)
        {

        }
    }

    /// <summary>
    /// Class for generating unique Ids based on Twitter Snowflake algorithm.
    /// </summary>
    /// <remarks>
    /// More details here: https://www.callicoder.com/distributed-unique-id-sequence-number-generator/
    /// </remarks>
    public static class UniqueIdGenerator
    {
        private const int MaxGeneratorId = 1024;
        private const int MaxSequenceId = 4096;

        // 13 apr 2020 (in Ticks / 10000)
        // can generate positive long numbers ~70 years from this date,
        // then a overflow starts
        private const long ModelStartEpoch = 63722332800000;
        private static long _generatorId = -1;

        private static long _sequenceId;
        private static long _lastSequenceId = -1;
        /// <summary>
        /// Время генерации последнего ИД (в мс)
        /// </summary>
        private static long _lastTimestamp = -1;
        
        private static readonly object LockObj = new object();
        
        /// <summary>
        /// Initializes generator.
        /// </summary>
        /// <param name="generatorId">Generator id. Should be unique among all created generators in the system. Method doesn't contains unique check.</param>
        public static void Initialize(int generatorId)
        {
            if (generatorId < 0 || generatorId > MaxGeneratorId)
                throw new ArgumentException($"{nameof(generatorId)} should be between 0 and {MaxGeneratorId}");

            _generatorId = generatorId;
        }
        
        /// <summary>
        /// Генерирует уникальный для всей системы ИД.
        /// </summary>
        /// <returns>Уникальный ИД</returns>
        /// <exception cref="InvalidOperationException">Если генератор не был инициализирован.</exception>
        public static long Generate()
        {
            if (_generatorId < 0)
                throw new InvalidOperationException($"Generator is not initialized. Please, call {nameof(Initialize)}");

            lock (LockObj)
            {
                var sequenceId = _sequenceId++ % MaxSequenceId;
                while (true)
                {
                    var timeStamp = DateTime.UtcNow.Ticks / 10000 - ModelStartEpoch;

                    if (timeStamp < _lastTimestamp)
                    {
                        // wait, it's time for correction
                        Thread.Sleep(200);
                        
                        timeStamp = DateTime.UtcNow.Ticks / 10000 - ModelStartEpoch;
                        if (timeStamp < _lastTimestamp)
                            throw new UniqueIdGenerationException($"Invalid system clock. Timestamp={timeStamp}, LastTimestamp={_lastTimestamp}");
                    }

                    if (timeStamp == _lastTimestamp && sequenceId < _lastSequenceId)
                    {
                        Thread.Sleep(0);
                        continue;
                    }

                    _lastTimestamp = timeStamp;
                    _lastSequenceId = sequenceId;

                    return timeStamp << 22 // 41 bit
                           | ((_generatorId % MaxGeneratorId) << 12) // 10 bit 
                           | (sequenceId % MaxSequenceId); // 12 bit
                }
            }
        }
    }
}
