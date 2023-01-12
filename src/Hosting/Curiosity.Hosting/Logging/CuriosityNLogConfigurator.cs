using System;
using NLog;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Class for configuring NLog.
    /// </summary>
    public class CuriosityNLogConfigurator
    {
        private const string MissingVarErrorMessageFormat = "NLog configuration file does not contain a \"{0}\" variable.";
            
        private const string LogOutputDirectoryVarName = "outputdir";

        private const string AppNameVarName = "appname";
        private const string MailToVarName = "mailto";
        private const string MailFromVarName = "mailfrom";

        private const string SmtpLoginVarName = "smtplogin";
        private const string SmtpPasswordVarName = "smtppassword";
        private const string SmtpServerVarName = "smtpserver";

        public CuriosityNLogConfigurator(string logConfigurationPath, Action<string> configureAction)
        {
            if (logConfigurationPath == null) throw new ArgumentNullException(nameof(logConfigurationPath));
            if (configureAction == null) throw new ArgumentNullException(nameof(configureAction));
            
            configureAction.Invoke(logConfigurationPath);
        }

        public CuriosityNLogConfigurator WithAppName(string appName)
        {
            if (String.IsNullOrWhiteSpace(appName))
                throw new ArgumentNullException(nameof(appName));
            
            if (!LogManager.Configuration.Variables.ContainsKey(AppNameVarName))
                throw new Exception(String.Format(MissingVarErrorMessageFormat, AppNameVarName));
            
            LogManager.Configuration.Variables[AppNameVarName] = appName;

            return this;
        }

        /// <summary>
        /// Sets output directory for log. NLog configuration file, specified at the previous step, should contain
        /// the definition for the variable named "<strong>logOutputDirectory</strong>" and use it for that method to take
        /// effect.
        /// </summary>
        /// <param name="logOutputDirectory">Path to the directory where log files should be stored. For example "<code>/var/log</code>".</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">when <paramref name="logOutputDirectory"/> is null</exception>
        /// <exception cref="Exception">when "<strong>logOutputDirectory</strong>" variable is not defined inside NLog
        /// configuration file.</exception>
        public CuriosityNLogConfigurator WithLogOutputDirectory(string logOutputDirectory)
        {
            if (logOutputDirectory == null)
                throw new ArgumentNullException(nameof(logOutputDirectory));

            if (!LogManager.Configuration.Variables.ContainsKey(LogOutputDirectoryVarName))
                throw new Exception(String.Format(MissingVarErrorMessageFormat, LogOutputDirectoryVarName));

            LogManager.Configuration.Variables[LogOutputDirectoryVarName] = logOutputDirectory;
                
            return this;
        }

        /// <summary>
        /// Sets the parameters for the mail logger target. NLog configuration file, specified at the previous step,
        /// should contain the definition for the following variables and use them for that method to take effect:
        /// <list type="bullet">
        /// <item><term>mailsubject:</term><description>subject of Mail that would be sent by logger.</description></item>
        /// <item><term>mailto:</term><description>email`s "From" field value.</description></item>
        /// <item><term>mailfrom:</term><description>where to send mail to.</description></item>
        /// 
        /// <item><term>smtplogin:</term><description>smtp server login (username).</description></item>
        /// <item><term>smtppassword:</term><description>smtp server password.</description></item>
        /// <item><term>smtpserver:</term><description>smtp server address.</description></item>
        /// </list>
        /// </summary>
        /// <param name="appName">Application name</param>
        /// <param name="loggerMailOptions">Mail configuration.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">when <paramref name="loggerMailOptions"/> is null</exception>
        /// <exception cref="Exception">when any of listed above variables is not defined inside NLog
        /// configuration file.</exception>
        public CuriosityNLogConfigurator WithMail(ILoggerMailOptions loggerMailOptions)
        {
            if (loggerMailOptions == null)
                throw new ArgumentNullException(nameof(loggerMailOptions));
                
            if (!LogManager.Configuration.Variables.ContainsKey(MailToVarName))
                throw new Exception(String.Format(MissingVarErrorMessageFormat, MailToVarName));
                
            if (!LogManager.Configuration.Variables.ContainsKey(MailFromVarName))
                throw new Exception(String.Format(MissingVarErrorMessageFormat, MailFromVarName));
                
            if (!LogManager.Configuration.Variables.ContainsKey(SmtpLoginVarName))
                throw new Exception(String.Format(MissingVarErrorMessageFormat, SmtpLoginVarName));
                
            if (!LogManager.Configuration.Variables.ContainsKey(SmtpPasswordVarName))
                throw new Exception(String.Format(MissingVarErrorMessageFormat, SmtpPasswordVarName));
                
            if (!LogManager.Configuration.Variables.ContainsKey(SmtpServerVarName))
                throw new Exception(String.Format(MissingVarErrorMessageFormat, SmtpServerVarName));
                
            // Set all options, except AppName, only if they are set in configuration.
            if (!String.IsNullOrWhiteSpace(loggerMailOptions.MailTo))
                LogManager.Configuration.Variables[MailToVarName] = loggerMailOptions.MailTo;
                
            if (!String.IsNullOrWhiteSpace(loggerMailOptions.EMailFrom))
                LogManager.Configuration.Variables[MailFromVarName] = loggerMailOptions.EMailFrom;
                
            if (!String.IsNullOrWhiteSpace(loggerMailOptions.SmtpLogin))
                LogManager.Configuration.Variables[SmtpLoginVarName] = loggerMailOptions.SmtpLogin;
                
            if (!String.IsNullOrWhiteSpace(loggerMailOptions.SmtpPassword))
                LogManager.Configuration.Variables[SmtpPasswordVarName] = loggerMailOptions.SmtpPassword;
                
            if (!String.IsNullOrWhiteSpace(loggerMailOptions.SmtpServer))
                LogManager.Configuration.Variables[SmtpServerVarName] = loggerMailOptions.SmtpServer;

            return this;
        }

        /// <summary>
        /// Reconfigures any existing loggers to use previously set parameters.
        /// </summary>
        public void Configure()
        {
            LogManager.ReconfigExistingLoggers();
        }
    }
}