using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Service for printing configuration to string.
    /// </summary>
    public class ConfigurationPrinter
    {
        private const string ModelNullText = "null";

        /// <summary>
        /// Returns string representation of configuration.
        /// </summary>
        /// <returns></returns>
        public string GetLog(ILoggableOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            return GetLog(options, 0, 30);
        }

        private string GetLog(ILoggableOptions? options, int offset, int defaultWidth, bool printSeparator = true)
        {
            var builder = new StringBuilder();
            if (printSeparator)
            {
                builder.AppendLine("-------------");
            }

            if (options == null) return ModelNullText;

            var type = options.GetType();
            foreach (var propertyInfo in type.GetProperties())
            {
                var propValue = propertyInfo.GetValue(options);
                var currentOffset = (offset + defaultWidth);
                var propName = $"{{{0}, {currentOffset}}}";
                var pro = String.Format(propName, propertyInfo.Name);
                builder.AppendLine($"{pro}: {ValueToString(propValue, currentOffset, 15)}");
            }

            // removed new line symbol - it's more beaty
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        private string ValueToString(object? propValue, int offset, int defaultWidth)
        {
            if (propValue == null) return ModelNullText;

            if (propValue is ILoggableOptions loggableOptions)
            {
                return GetLog(loggableOptions, offset, defaultWidth);
            }

            if (propValue is ILoggableOptions[] loggableOption)
            {
                var log = new StringBuilder();
                var printSeparator = true;
                foreach (var option in loggableOption)
                {
                    log.AppendLine(GetLog(option, offset, defaultWidth, printSeparator));
                    printSeparator = false;
                }

                return log.ToString();
            }

            if (propValue is ICollection collection)
            {
                var arr = new List<object>();
                var enumerator = collection.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    arr.Add(enumerator.Current);
                }

                return "[" + string.Join(",", arr.ToArray()) + "]";
            }

            return propValue.ToString();
        }
    }
}