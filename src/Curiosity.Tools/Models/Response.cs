using System;
using System.Collections.Generic;

namespace Curiosity.Tools.Models
{
    /// <summary>
    /// Response without body.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// True if query executed without errors.
        /// </summary>
        public bool IsSuccess { get; }

        /// <summary>
        /// List of errors. Empty if success.
        /// </summary>
        public IReadOnlyList<Error> Errors { get; }

        /// <summary>
        /// Use when success
        /// </summary>
        protected Response(bool isSuccess, IReadOnlyList<Error> errors)
        {
            IsSuccess = isSuccess;
            Errors = errors;
        }

        /// <summary>
        /// Creates response for successful case.
        /// </summary>
        public static Response Successful()
        {
            return new Response(true, Array.Empty<Error>());
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response Failed(IReadOnlyList<Error> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            return new Response(false, errors);
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response Failed(Error error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            return new Response(false, new[] { error });
        }
    }

    /// <summary>
    /// Response with body.
    /// </summary>
    public class Response<T> : Response
    {
        /// <summary>
        /// Body of response
        /// </summary>
        public T Body { get; }

        protected Response(T body, bool isSuccess, IReadOnlyList<Error> errors) : base(isSuccess, errors)
        {
            Body = body;
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response<T> Successful(T body)
        {
            return new Response<T>(body, true, Array.Empty<Error>());
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public new static Response<T> Failed(IReadOnlyList<Error> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            return new Response<T>(default!, false, errors);
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public new static Response<T> Failed(Error error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            return new Response<T>(default!, false, new[] { error });
        }
    }
}
