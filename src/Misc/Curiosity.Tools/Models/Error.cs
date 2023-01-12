using System;

namespace Curiosity.Tools
{
    /// <summary>
    /// Model of business logic error
    /// </summary>
    public class Error
    {
        /// <summary>
        /// Code of business logic error
        /// </summary>
        public int Code { get; }

        /// <summary>
        /// Description of business logic error
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Model key
        /// </summary>
        public string? Key { get; }

        /// <summary>
        /// Creates a new Error
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="key"></param>
        public Error(int code, string description, string? key = null)
        {
            if (String.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));

            Code = code;
            Description = description;
            Key = key;
        }
    }
}
