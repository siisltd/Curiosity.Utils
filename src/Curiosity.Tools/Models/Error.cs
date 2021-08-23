using System;

namespace Curiosity.Tools.Models
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
        public Error(int code, string description)
        {
            if (String.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));

            Code = code;
            Description = description;
            Key = null;
        }

        /// <summary>
        /// Creates a new Error
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="key"></param>
        public Error(int code, string description, string? key)
        {
            if (String.IsNullOrWhiteSpace(description))
                throw new ArgumentNullException(nameof(description));

            Code = code;
            Description = description;
            Key = key;
        }
    }
}
