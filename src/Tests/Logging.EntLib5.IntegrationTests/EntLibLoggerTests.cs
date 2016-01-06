using System;
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Logging.EntLib5.IntegrationTests
{
    public class EntLibLoggerTests
    {
        protected const string LogLineFormatter = "{0}|{1}|{2}";

        protected LogWriter CreateLogWriter(string logFilename, Tuple<string, SourceLevels>[] categoryAndSourceLevels = null, bool isEnable = true)

        {
            DictionaryConfigurationSource configSource = CreateLoggingConfig(logFilename, categoryAndSourceLevels, isEnable);
            EnterpriseLibraryContainer.Current = EnterpriseLibraryContainer.CreateDefaultContainer(configSource);
            LogWriterFactory lwf = new LogWriterFactory();
            LogWriter logWriter = lwf.Create();
            return logWriter;
        }

        private DictionaryConfigurationSource CreateLoggingConfig(string logFilename, Tuple<string, SourceLevels>[] categoryAndSourceLevels, bool isEnable)
        {
            ConfigurationSourceBuilder builder = new ConfigurationSourceBuilder();

            ILoggingConfigurationOptions options = builder.ConfigureLogging().WithOptions.DoNotRevertImpersonation();
            ILoggingConfigurationFilterLogEnabled filterEnableOrDisable = options.FilterEnableOrDisable("Logging Enabled Filter");
            options = filterEnableOrDisable;
            if (isEnable)
                options = filterEnableOrDisable.Enable();

            IFormatterBuilder formatterBuilder = new FormatterBuilder().TextFormatterNamed("Text Formatter").UsingTemplate(string.Format(LogLineFormatter, "{severity}", "{category}", "{message}"));

            if (categoryAndSourceLevels != null && categoryAndSourceLevels[0].Item1 != null)
            {
                foreach (var categoryAndSourceLevel in categoryAndSourceLevels)
                {
                    options.LogToCategoryNamed(categoryAndSourceLevel.Item1).WithOptions.SetAsDefaultCategory().ToSourceLevels(categoryAndSourceLevel.Item2)
                        .SendTo.FlatFile("Log File").WithHeader(String.Empty).WithFooter(string.Empty)
                        .FormatWith(formatterBuilder)
                        .ToFile(logFilename);
                }
            }

            DictionaryConfigurationSource configSource = new DictionaryConfigurationSource();
            builder.UpdateConfigurationWithReplace(configSource);
            return configSource;
        }        
    }
}