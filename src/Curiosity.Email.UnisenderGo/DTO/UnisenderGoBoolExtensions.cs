namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Extensions methods for bool.
    /// </summary>
    internal static class UnisenderGoBoolExtensions
    {
        /// <summary>
        /// Converts .NET bool value to UnisenderGo bool value.
        /// </summary>
        public static int ToUnisenderGoBool(this bool boolValue)
        {
            return boolValue
                ? 1
                : 0;
        }
    }
}
