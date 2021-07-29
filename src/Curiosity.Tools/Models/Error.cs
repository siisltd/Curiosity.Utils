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
        /// Create a new Error
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        public Error(int code, string description)
        {
            Code = code;
            Description = description;
            Key = null;
        }

        /// <summary>
        /// Create a new Error
        /// </summary>
        /// <param name="code"></param>
        /// <param name="description"></param>
        /// <param name="key"></param>
        public Error(int code, string description, string? key)
        {
            Code = code;
            Description = description;
            Key = key;
        }
    }
}
