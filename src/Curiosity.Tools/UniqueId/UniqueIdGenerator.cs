using System;
using System.Threading;

namespace Curiosity.Tools
{
    /// <summary>
    /// Генератор уникальных ИД на базе Twitter Snowflake.
    /// </summary>
    /// <remarks>
    /// Подробнее можно почитать тут: https://www.callicoder.com/distributed-unique-id-sequence-number-generator/
    /// </remarks>
    public static class UniqueIdGenerator
    {
        // 13 apr 2020 (in Ticks / 10000)
        private const long StartEpoch = 63722332800000;
        private static long _generatorId = -1;

        private static long _sequenceId;
        private static long _lastSequenceId = -1;
        /// <summary>
        /// Время генерации последнего ИД (в мс)
        /// </summary>
        private static long _lastTimestamp = -1;
        
        private static readonly object LockObj = new object();
        
        /// <summary>
        /// Инициализирует генератор
        /// </summary>
        /// <param name="generatorId">ИД генератора. Должен быть уникальным среди всех запущенных приложений. Проверки уникальности нет.</param>
        /// <exception cref="ArgumentException">Если меньше 0 или больше максимального значения.</exception>
        public static void Initialize(int generatorId)
        {
            if (generatorId < 0 || generatorId > 1024)
                throw new ArgumentException($"{nameof(generatorId)} should be between 0 and 1024");

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
                var sequenceId = (_sequenceId++) % 4096;
                while (true)
                {
                    var timeStamp = (System.DateTime.UtcNow.Ticks / 10000) - StartEpoch;

                    if (timeStamp < _lastTimestamp)
                    {
                        // ждём, вдруг время корректировалось
                        Thread.Sleep(200);
                        
                        timeStamp = (System.DateTime.UtcNow.Ticks / 10000) - StartEpoch;
                        if (timeStamp < _lastTimestamp)
                            throw new InvalidOperationException("Invalid system clock");
                    }

                    if (timeStamp == _lastTimestamp && sequenceId < _lastSequenceId)
                    {
                        Thread.Sleep(0);
                        continue;
                    }

                    _lastTimestamp = timeStamp;
                    _lastSequenceId = sequenceId;

                    return timeStamp << 22 // 41 bit
                           | ((_generatorId % 1024) << 12) // 10 bit 
                           | (sequenceId % 4096); // 12 bit
                }
            }
        }
    }
}