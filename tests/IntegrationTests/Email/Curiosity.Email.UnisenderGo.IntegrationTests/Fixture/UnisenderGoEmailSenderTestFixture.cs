using Curiosity.Configuration;

namespace Curiosity.Email.UnisenderGo.IntegrationTests
{
    /// <summary>
    /// Fixture for testing sending email via UnisenderGo.
    /// </summary>
    public class UnisenderGoEmailSenderTestFixture
    {
        /// <summary>
        /// UnisenderGo options.
        /// </summary>
        public UnisenderGoEmailOptions UnisenderGoEmailOptions { get; }

        /// <inheritdoc cref="UnisenderGoEmailSenderTestFixture"/>
        public UnisenderGoEmailSenderTestFixture()
        {
            var configurationProvider = new YamlConfigurationProvider<TestOptions>(null);
            var configuration = configurationProvider.GetConfiguration();
            
            UnisenderGoEmailOptions = configuration.UnisenderGo;
        }

        /// <summary>
        /// Dummy class for reading options from config file for <see cref="UnisenderGoEmailSenderTestFixture"/>.
        /// </summary>
        private class TestOptions
        {
            /// <summary>
            /// UnisenderGo options.
            /// </summary>
            public UnisenderGoEmailOptions UnisenderGo { get; set; } = null!;
        }
    }
}
