using System;
using System.Collections.Generic;

namespace Curiosity.Tools
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
        /// Constructor for serialization. It's better to use static factory methods.
        /// </summary>
        public Response(bool isSuccess, IReadOnlyList<Error>? errors = null)
        {
            IsSuccess = isSuccess;
            if (!isSuccess && (errors == null || errors.Count == 0))
                throw new ArgumentException("Errors should be specified for failed response", nameof(errors));

            Errors = errors ?? Array.Empty<Error>();
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

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response Failed(int errorCode, string errorDescription)
        {
            var error = new Error(errorCode, errorDescription);
            return new Response(false, new[] { error });
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response<T> Successful<T>(T body)
        {
            return new Response<T>(body, true, Array.Empty<Error>());
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public  static Response<T> Failed<T>(IReadOnlyList<Error> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            return new Response<T>(default!, false, errors);
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response<T> Failed<T>(Error error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            return new Response<T>(default!, false, new[] { error });
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response<T> Failed<T>(Error error, T body)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));
            if (body == null) throw new ArgumentNullException(nameof(body));

            return new Response<T>(body, false, new[] { error });
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public static Response<T> Failed<T>(int errorCode, string errorDescription)
        {
            var error = new Error(errorCode, errorDescription);
            return new Response<T>(default!, false, new[] { error });
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

        /// <inheritdoc />
        public Response(T body, bool isSuccess, IReadOnlyList<Error>? errors) : base(isSuccess, errors)
        {
            Body = body;
        }
    }
}
