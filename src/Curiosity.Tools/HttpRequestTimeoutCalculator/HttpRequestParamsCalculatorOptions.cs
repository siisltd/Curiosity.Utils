using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Tools.HttpRequestTimeoutCalculator
{
    /// <summary>
    /// Options for <see cref="IHttpRequestParamsCalculator"/>
    /// </summary>
    public class HttpRequestParamsCalculatorOptions : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// Data rate in MegaByte per second
        /// </summary>
        public int DataRateMBps { get; set; } = 8;

        /// <summary>
        /// Multiplier for timeout calculation 
        /// </summary>
        /// <remarks>
        /// Multiplier used for more precisely calculation because throughput rate is not permanent and
        /// different delays are expected during HTTP request
        /// Used formula: size / <see cref="DataRateMBps"/> * <see cref="TimeoutMultiplier"/>
        /// </remarks>
        public double TimeoutMultiplier { get; set; } = 1.5;

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(DataRateMBps)}={DataRateMBps};{nameof(TimeoutMultiplier)}={TimeoutMultiplier}";
        }

        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(TimeoutMultiplier < 0, nameof(TimeoutMultiplier), "should be greater than 0");
            errors.AddErrorIf(DataRateMBps < 0, nameof(DataRateMBps), "should be greater than 0");
            
            return errors;
        }
    }
}