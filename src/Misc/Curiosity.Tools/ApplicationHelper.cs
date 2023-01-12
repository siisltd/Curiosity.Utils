using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Curiosity.Configuration;

namespace Curiosity.Tools
{
    /// <summary>
    /// Useful staff for application.
    /// </summary>
    public static class ApplicationHelper
    {
        /// <summary>
        /// Sets a default culture for application.
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void SetDefaultCulture(CultureOptions options)
        {
            if (options == null) 
                throw new ArgumentNullException(nameof(options));

            options.AssertValid();

            var culture = new CultureInfo(options.DefaultCulture);
            
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
        }

        /// <summary>
        /// When the app environment is development, then setts current directory from executing assembly.
        /// </summary>
        /// <remarks>
        /// Useful when you want to write logs no directory where bins are located.
        /// </remarks>
        public static void ChangeCurrentDirectoryForDevelopment()
        {
            if (!EnvironmentHelper.IsDevelopmentEnvironment()) 
                return;
            
            var executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!String.IsNullOrEmpty(executingLocation))
            {
                Directory.SetCurrentDirectory(executingLocation);
            }
        }

        /// <summary>
        /// Returns application version.
        /// </summary>
        /// <returns>Application version</returns>
        public static string GetAssemblyVersion()
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version ?? Assembly.GetExecutingAssembly()?.GetName().Version; 
            return version?.ToString() ?? throw new InvalidOperationException("Can't get executing assembly.");
        }
    }
}
