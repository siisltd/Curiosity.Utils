using System;

namespace Curiosity.Tools.HttpRequestTimeoutCalculator
{
    /// <summary>
    /// Class for calculating different http request parameters.
    /// </summary>
    public interface IHttpRequestParamsCalculator
    {
        /// <summary>
        /// Calculate downloading/uploading rate in MBps
        /// </summary>
        /// <param name="sizeInBytes">Size of downloaded or uploaded file in bytes</param>
        /// <param name="totalSeconds">Spent for file download/upload</param>
        /// <exception cref="ArgumentOutOfRangeException">If any of params are less or equal than 0</exception>
        /// <returns>Rate in MBps</returns>
        double CalculateThroughputRateInMBps(long sizeInBytes, double totalSeconds);
        
        /// <summary>
        /// Calculate timeout fot HTTP request depending on file size and current throughput rate
        /// </summary>
        /// <param name="sizeInBytes">Size of downloaded/uploaded file in bytes</param>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="sizeInBytes"/> is less or equal than 0</exception>
        /// <returns>Timeout for request</returns>
        /// <remarks>
        /// Throughput rate is passed via <see cref="HttpRequestParamsCalculatorOptions"/>
        /// </remarks>
        TimeSpan CalculateTimeout(long sizeInBytes);
    }
}