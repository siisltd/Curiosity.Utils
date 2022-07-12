namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Extensions methods for bool.
    /// </summary>
    internal static class UnisenderGoExtensions
    {
        /// <summary>
        /// Converts bool? to int?.
        /// </summary>
        public static int? ToNullableInt(this bool? boolValue)
        {
            return boolValue.HasValue
                ? boolValue.Value
                    ? 1
                    : 0
                : (int?) null;
        }
    }
}
