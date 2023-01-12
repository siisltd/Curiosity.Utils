using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.SFTP
{
    /// <summary>
    /// Options for <see cref="ISftpClient"/>.
    /// </summary>
    public class SftpClientOptions : ILoggableOptions, IValidatableOptions
    {
        private const string ShouldBeGreaterOrEqualZeroErrorMessage = "should be greater or equal to 0";

        /// <summary>
        /// SSH server.
        /// </summary>
        public string SshServer { get; set; } = null!;

        /// <summary>
        /// SSH port.
        /// </summary>
        public int SshPort { get; set; }

        /// <summary>
        /// SSH login.
        /// </summary>
        public string SshLogin { get; set; } = null!;

        /// <summary>
        /// SSH password.
        /// </summary>
        public string SshPassword { get; set; } = null!;

        /// <summary>
        /// Absolute path to SSH key.
        /// </summary>
        public string SshPrivateKeyPath { get; set; } = null!;

        /// <summary>
        /// Passphrase for SSH key (non required).
        /// </summary>
        public string SshPrivateKeyPassphrase { get; set; } = null!;

        /// <summary>
        /// Count of uploading retry.
        /// </summary>
        public int RetryCount { get; set; } = 3;

        /// <summary>
        /// Timeout between retries, sec.
        /// </summary>
        public int RetryTimeoutSec { get; set; } = 5;

        /// <summary>
        /// Need to check the connection at the start of the app.
        /// </summary>
        public bool CheckOnStart { get; set; } = true;

        /// <inheritdoc cref="IValidatableOptions" />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(SshServer), nameof(SshServer), "can not be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(SshLogin), nameof(SshLogin), "can not be empty");
            errors.AddErrorIf(SshPort < 0, nameof(SshPort), ShouldBeGreaterOrEqualZeroErrorMessage);
            errors.AddErrorIf(String.IsNullOrWhiteSpace(SshPassword) && String.IsNullOrWhiteSpace(SshPrivateKeyPath), "No auth method", $"specify {nameof(SshPassword)} or {nameof(SshPrivateKeyPath)}");
            errors.AddErrorIf(RetryCount < 0, nameof(RetryCount), ShouldBeGreaterOrEqualZeroErrorMessage);
            errors.AddErrorIf(RetryTimeoutSec < 0, nameof(RetryTimeoutSec), ShouldBeGreaterOrEqualZeroErrorMessage);

            return errors;
        }
    }
}