using System.Runtime.CompilerServices;

namespace Curiosity.Utils.FileDataReaderWriters;

public static partial class Guard
    {
        /// <summary>
        /// Проверяет, что параметр не null.
        /// </summary>
        /// <param name="parameter">Значение параметра.</param>
        /// <param name="parameterName">Название параметра.</param>
        /// <typeparam name="T">Тип параметра.</typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertNotNull<T>(T? parameter, string parameterName) where T: class
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }
        
        /// <summary>
        /// Проверяет, что параметр не null.
        /// </summary>
        /// <param name="parameter">Значение параметра.</param>
        /// <param name="parameterName">Название параметра.</param>
        /// <typeparam name="T">Тип параметра.</typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertHasValue<T>(Nullable<T> parameter, string parameterName) where T: struct
        {
            if (!parameter.HasValue)
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Проверяет, что строка не пустая.
        /// </summary>
        /// <param name="parameter">Значение параметра</param>
        /// <param name="parameterName">Название параметра</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertNotEmpty(string? parameter, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(parameter))
                throw new ArgumentNullException(parameterName);
        }

        /// <summary>
        /// Проверяет, что значение находится в заданном диапазоне.
        /// </summary>
        /// <param name="parameter">Значение параметра</param>
        /// <param name="minValue">Минимальное допустимое значение</param>
        /// <param name="maxValue">Максимальное допустимое значение</param>
        /// <param name="parameterName">Название параметра</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertInRange(int parameter, int minValue, int maxValue, string parameterName)
        {
            if (parameter < minValue || parameter > maxValue)
                throw new ArgumentOutOfRangeException(parameterName);
        }

        /// <summary>
        /// Проверяет, что значение не отрицательно.
        /// </summary>
        /// <param name="parameter">Значение параметра</param>
        /// <param name="parameterName">Название параметра</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertNotNegative(int? parameter, string parameterName)
        {
            if (!parameter.HasValue) return;
            AssertNotNegative(parameter.Value, parameterName);
        }

        /// <summary>
        /// Проверяет, что значение не отрицательно.
        /// </summary>
        /// <param name="parameter">Значение параметра</param>
        /// <param name="parameterName">Название параметра</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertNotNegative(int parameter, string parameterName)
        {
            if (parameter < 0)
                throw new ArgumentOutOfRangeException(parameterName);
        }
    }