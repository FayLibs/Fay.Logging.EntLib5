using System;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Fay.Logging.EntLib5
{
    /// <summary>
    /// Simple string message implementation of the <see cref="DelegateLogger{TMetadata}"/> for use with Microsoft Enterprise Library Logging Framework. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This is useful if using Enterprise Library Logging Application Block with a single category, without any other advance features, and only want to long simple string based messages.
    /// </remarks>
    public sealed class EntLibSimpleMessageLogger : EntLibLogger<string>
    {       
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EntLibSimpleMessageLogger"/> class.
        /// </summary>
        /// <param name="logWriter">The log writer.</param>
        public EntLibSimpleMessageLogger(LogWriter logWriter) : base(logWriter)
        {           
        }

        /// <summary>
        /// Determines whether the provided <paramref name="severity"/> is in scope, <paramref name="messageDelegate"/> is ignored and defaults to null.
        /// </summary>
        /// <remarks>
        /// You can check if logging is enabled by checking if <see cref="LogSeverity.Off"/> is in scope, if returns <c>true</c> then logging is disabled; otherwise logging is enabled.
        /// This only checks against the default/first category, if the category doesn't exist for some reason it will return <c>false</c>.
        /// </remarks>
        /// <param name="severity">The severity to check if it is in scope..</param>
        /// <param name="messageDelegate">The message delegate, which is ignored.</param>
        /// <returns><c>true</c> if <paramref name="severity"/> is in scope; otherwise, <c>false</c>.</returns>
        public override bool IsSeverityInScope(LogSeverity severity, Func<string> messageDelegate = null)
        {
            if (severity == LogSeverity.Off && !MyLogWriter.IsLoggingEnabled())
                return true;

            if (!string.IsNullOrWhiteSpace(DefaultCategory) && CategoryMinimumSeverityMap.ContainsKey(DefaultCategory))
            {
                LogSeverity categorySeverity = CategoryMinimumSeverityMap[DefaultCategory];
                if (severity == LogSeverity.Off && categorySeverity == LogSeverity.Off)
                    return true;
                return severity <= categorySeverity && categorySeverity >= LogSeverity.Off && severity != LogSeverity.Off;
            }
            
            return false;
        }

        /// <summary>
        /// Simple implementation to wrap the provided <paramref name="messageDelegate" /> and <paramref name="ex" /> within a new delegate message that blends them together.
        /// </summary>
        /// <param name="messageDelegate">The message delegate to wrap.</param>
        /// <param name="ex">The exception to wrap.</param>
        /// <returns>A new delegate that will return the results of the <paramref name="messageDelegate" /> and <paramref name="ex" /></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected override Func<string> InjectExceptionIntoMessageDelegate(Func<string> messageDelegate, Exception ex)
        {
            if (ex == null)
                return messageDelegate;

            Func<string> msg = () =>
            {
                Func<string> localMessageDelegate = messageDelegate;
                string originalMessage = string.Empty;
                
                if (localMessageDelegate != null)
                    originalMessage = localMessageDelegate();
                
                return MergeMessageWithException(originalMessage, ex);
            };

            return msg;
        }

        /// <summary>
        /// Implementation specific method that actually writes to underlying logger.
        /// </summary>
        /// <param name="severity">The <see cref="T:Gnomesoft.Logging.LogSeverity" /> to use.</param>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void Write(LogSeverity severity, Func<string> messageDelegate)
        {
            if (!IsSeverityInScope(severity))
                return;

            string message = messageDelegate?.Invoke();

            if (string.IsNullOrEmpty(message))
                return;

            LogEntry entry = new LogEntry
            {
                Severity = LogSeverityToTraceEventTypeMap[severity],
                Message = message,
            };
            entry.Categories.Add(DefaultCategory);

            MyLogWriter.Write(entry);
        }
    }
}
