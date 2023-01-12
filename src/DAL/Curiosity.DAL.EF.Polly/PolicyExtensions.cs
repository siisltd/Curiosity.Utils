using System;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Curiosity.DAL.EF.Polly
{
    /// <summary>
    /// Extensions methods for <see cref="Polly"/>.
    /// </summary>
    public static class PolicyExtensions
    {
        /// <summary>
        /// Prebuild policy for handling of all concurrency exception on updating entities.
        /// </summary>
        public static PolicyBuilder GetHandleDbConcurrencyExceptionsPolicy()
        {
            return Policy.Handle<DbUpdateConcurrencyException>()
                .Or<DbUpdateException>()
                .Or<InvalidOperationException>(ex =>
                    ex.InnerException != null && (ex.InnerException is DbUpdateException ||
                                                  ex.InnerException is DbUpdateConcurrencyException));
        }
    }
}