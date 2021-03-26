using System;
using Microsoft.AspNetCore.DataProtection;

namespace Curiosity.Tools.Web.DataProtection
{
    public class SecureLessDataProtectionProvider : IDataProtectionProvider
    {
        private readonly IDataProtectionProvider _innerDataProtectionProvider;
        private readonly DataProtectionOptions _options;

        public SecureLessDataProtectionProvider(IDataProtectionProvider innerDataProtectionProvider, DataProtectionOptions options)
        {
            _innerDataProtectionProvider = innerDataProtectionProvider ?? throw new ArgumentNullException(nameof(innerDataProtectionProvider));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public IDataProtector CreateProtector(string purpose)
        {
            // if you do not need protection, then we will return the stub
            return _options.IsEnabled
                ? _innerDataProtectionProvider.CreateProtector(purpose)
                : new SecureLessDataProtector(this);
        }
    }
}