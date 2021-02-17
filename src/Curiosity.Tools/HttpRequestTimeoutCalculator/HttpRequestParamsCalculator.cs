using System;
using Curiosity.Configuration;

namespace Curiosity.Tools.HttpRequestTimeoutCalculator
{
    /// <inheritdoc />
    public class HttpRequestParamsCalculator : IHttpRequestParamsCalculator
    {
        /// <summary>
        /// Default timeout
        /// </summary>
        private const int DefaultTimeout = 100;
        
        /// <summary>
        /// Options for calculator
        /// </summary>
        private readonly HttpRequestParamsCalculatorOptions _options;
        
        private readonly int _bytesPerSecond;
        
        public HttpRequestParamsCalculator(HttpRequestParamsCalculatorOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            
            options.AssertValid();
            
            _bytesPerSecond = options.DataRateMBps * (int)1E6;
        }

        /// <inheritdoc />
        public double CalculateThroughputRateInMBps(long sizeInBytes, double totalSeconds)
        {
            if (sizeInBytes < 0) throw new ArgumentOutOfRangeException( nameof(sizeInBytes), $"{nameof(sizeInBytes)} should be greater than 0");
            
            if (totalSeconds < 0) throw new ArgumentOutOfRangeException( nameof(totalSeconds), $"{nameof(totalSeconds)} should be greater than 0");
            
            return sizeInBytes / totalSeconds / 1E6;
        }

        /// <inheritdoc />
        public TimeSpan CalculateTimeout(long sizeInBytes)
        {
            if (sizeInBytes < 0) throw new ArgumentOutOfRangeException( nameof(sizeInBytes), $"{nameof(sizeInBytes)} should be greater than 0");
            
            // ReSharper disable once PossibleLossOfFraction
            var seconds = Math.Max(DefaultTimeout, (long) (sizeInBytes / _bytesPerSecond *_options.TimeoutMultiplier));

            return TimeSpan.FromSeconds(seconds);
        }
    }
}