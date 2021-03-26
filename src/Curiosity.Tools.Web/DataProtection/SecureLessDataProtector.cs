using System;
using Microsoft.AspNetCore.DataProtection;

namespace Curiosity.Tools.Web.DataProtection
{
    /// <summary>
    /// A <see cref="IDataProtector"/> simulator that doesn't encrypt anything.
    /// A stub to keep the logs clean.
    /// </summary>
    public class SecureLessDataProtector : IDataProtector
    {
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public SecureLessDataProtector(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
        }

        /// <inheritdoc />
        public IDataProtector CreateProtector(string purpose)
        {
            return _dataProtectionProvider.CreateProtector(purpose);
        }

        /// <inheritdoc />
        public byte[] Protect(byte[] plaintext) => plaintext;

        /// <inheritdoc />
        public byte[] Unprotect(byte[] protectedData) => protectedData;
    }
}