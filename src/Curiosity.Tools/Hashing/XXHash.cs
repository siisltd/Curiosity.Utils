using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Curiosity.Tools.Hashing
{
    /// <summary>
    /// Implements xxHash as specified at https://code.google.com/p/xxhash/source/browse/trunk/xxhash.c and https://code.google.com/p/xxhash/.
    /// </summary>
    public class XXHash : HashFunctionBase
    {
        /// <summary>
        /// Seed value for hash calculation.
        /// </summary>
        /// <value>
        /// The seed value for hash calculation.
        /// </value>
        public ulong InitVal { get; set; }

        /// <summary>
        /// The list of possible hash sizes that can be provided to the <see cref="XXHash" /> constructor.
        /// </summary>
        /// <value>
        /// The list of valid hash sizes.
        /// </value>
        public static IEnumerable<int> ValidHashSizes => _validHashSizes;

        private static readonly IList<uint> _primes32 = 
            new[] {
                2654435761U,
                2246822519U,
                3266489917U,
                 668265263U,
                 374761393U
            };

        private static readonly IList<ulong> _primes64 = 
            new[] {
                11400714785074694791UL,
                14029467366897019727UL,
                 1609587929392839161UL,
                 9650029242287828579UL,
                 2870177450012600261UL
            };

        private static readonly IEnumerable<int> _validHashSizes = new[] { 32, 64 };

        /// <remarks>
        /// Defaults <see cref="InitVal" /> to 0.  <inheritdoc cref="XXHash(ulong)" />
        /// </remarks>
        /// <inheritdoc cref="XXHash(ulong)" />
        public XXHash()
            : this(0U)
        {
        }

        /// <remarks>
        /// Defaults <see cref="InitVal" /> to 0.
        /// </remarks>
        /// <inheritdoc cref="XXHash(int,ulong)" />
        public XXHash(int hashSize)
            :this (hashSize, 0U)
        {
        }

        /// <remarks>
        /// Defaults <see cref="HashFunctionBase.HashSize" /> to 32.
        /// </remarks>
        /// <inheritdoc cref="XXHash(int,ulong)" />
        public XXHash(ulong initVal)
            : this(32, initVal)
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="XXHash" /> class.
        /// </summary>
        /// <param name="hashSize"><inheritdoc cref="HashFunctionBase.HashSize" /></param>
        /// <param name="initVal"><inheritdoc cref="InitVal" /></param>
        /// <exception cref="System.ArgumentOutOfRangeException">hashSize;hashSize must be contained within xxHash.ValidHashSizes</exception>
        /// <inheritdoc cref="HashFunctionBase(int)" />
        public XXHash(int hashSize, ulong initVal)
            : base(hashSize)
        {
            if (!ValidHashSizes.Contains(hashSize))
                throw new ArgumentOutOfRangeException(nameof(hashSize), "hashSize must be contained within xxHash.ValidHashSizes");

            InitVal = initVal;
        }

        /// <exception cref="System.InvalidOperationException">HashSize set to an invalid value.</exception>
        /// <inheritdoc />
        protected override byte[] ComputeHashInternal(UnifiedData data)
        {
            byte[]? hash = null;

            switch (HashSize)
            {
                case 32:
                {
                    var h = (uint) InitVal + _primes32[4];

                    int dataCount = 0;
                    byte[]? remainder = null;


                    var initValues = new[] {
                        (uint) InitVal + _primes32[0] + _primes32[1],
                        (uint) InitVal + _primes32[1],
                        (uint) InitVal,
                        (uint) InitVal - _primes32[0]
                    };

                    data.ForEachGroup(16, 
                        (dataGroup, position, length) => {
                            for (int x = position; x < position + length; x += 16)
                            {
                                for (var y = 0; y < 4; ++y)
                                {
                                    initValues[y] += BitConverter.ToUInt32(dataGroup, x + y * 4) * _primes32[1];
                                    initValues[y] = initValues[y].RotateLeft(13);
                                    initValues[y] *= _primes32[0];
                                }
                            }

                            dataCount += length;
                        },
                        (remainderData, position, length) => {
                            remainder = new byte[length];
                            Array.Copy(remainderData, position, remainder, 0, length);

                            dataCount += length;
                        });


                    PostProcess(ref h, initValues, dataCount, remainder);

                    hash = BitConverter.GetBytes(h);
                    break;
                }

                case 64:
                {
                     var h = InitVal + _primes64[4];

                    int dataCount = 0;
                    byte[]? remainder = null;

                    var initValues = new[] {
                        InitVal + _primes64[0] + _primes64[1],
                        InitVal + _primes64[1],
                        InitVal,
                        InitVal - _primes64[0]
                    };


                    data.ForEachGroup(32, 
                        (dataGroup, position, length) => {

                            for (var x = position; x < position + length; x += 32)
                            {
                                for (var y = 0; y < 4; ++y)
                                {
                                    initValues[y] += BitConverter.ToUInt64(dataGroup, x + y * 8) * _primes64[1];
                                    initValues[y] = initValues[y].RotateLeft(31);
                                    initValues[y] *= _primes64[0];
                                }
                            }

                            dataCount += length;
                        },
                        (remainderData, position, length) => {
                            remainder = new byte[length];
                            Array.Copy(remainderData, position, remainder, 0, length);

                            dataCount += length;
                        });


                    PostProcess(ref h, initValues, dataCount, remainder);

                    hash = BitConverter.GetBytes(h);
                    break;
                }
            }

            return hash ?? Array.Empty<byte>();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PostProcess(ref uint h, uint[] initValues, int dataCount, byte[]? remainder)
        {
            if (dataCount >= 16)
            {
                h = initValues[0].RotateLeft(1) + 
                    initValues[1].RotateLeft(7) + 
                    initValues[2].RotateLeft(12) + 
                    initValues[3].RotateLeft(18);
            }


            h += (uint) dataCount;

            if (remainder != null)
            {
                // In 4-byte chunks, process all process all full chunks
                for (int x = 0; x < remainder.Length / 4; ++x)
                {
                    h += BitConverter.ToUInt32(remainder, x * 4) * _primes32[2];
                    h  = h.RotateLeft(17) * _primes32[3];
                }


                // Process last 4 bytes in 1-byte chunks (only runs if data.Length % 4 != 0)
                for (int x = remainder.Length - remainder.Length % 4; x < remainder.Length; ++x)
                {
                    h += remainder[x] * _primes32[4];
                    h  = h.RotateLeft(11) * _primes32[0];
                }
            }

            h ^= h >> 15;
            h *= _primes32[1];
            h ^= h >> 13;
            h *= _primes32[2];
            h ^= h >> 16;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void PostProcess(ref ulong h, ulong[] initValues, int dataCount, byte[]? remainder)
        {
            if (dataCount >= 32)
            {
                h = initValues[0].RotateLeft(1) +
                    initValues[1].RotateLeft(7) +
                    initValues[2].RotateLeft(12) +
                    initValues[3].RotateLeft(18);


                for (var x = 0; x < initValues.Length; ++x)
                {
                    initValues[x] *= _primes64[1];
                    initValues[x] = initValues[x].RotateLeft(31);
                    initValues[x] *= _primes64[0];

                    h ^= initValues[x];
                    h = h * _primes64[0] + _primes64[3];
                }
            }

            h += (ulong) dataCount;

            if (remainder != null)
            { 
                // In 8-byte chunks, process all full chunks
                for (int x = 0; x < remainder.Length / 8; ++x)
                {
                    h ^= (BitConverter.ToUInt64(remainder, x * 8) * _primes64[1]).RotateLeft(31) * _primes64[0];
                    h  = h.RotateLeft(27) * _primes64[0] + _primes64[3];
                }


                // Process a 4-byte chunk if it exists
                if (remainder.Length % 8 > 4)
                {
                    h ^= BitConverter.ToUInt32(remainder, remainder.Length - remainder.Length % 8) * _primes64[0];
                    h  = h.RotateLeft(23) * _primes64[1] + _primes64[2];
                }

                // Process last 4 bytes in 1-byte chunks (only runs if data.Length % 4 != 0)
                for (int x = remainder.Length - remainder.Length % 4; x < remainder.Length; ++x)
                {
                    h ^= remainder[x] * _primes64[4];
                    h  = h.RotateLeft(11) * _primes64[0];
                }
            }


            h ^= h >> 33;
            h *= _primes64[1];
            h ^= h >> 29;
            h *= _primes64[2];
            h ^= h >> 32;
        }
    }
}