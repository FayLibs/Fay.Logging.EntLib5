using System;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Fay.Logging.EntLib5
{
    /// <summary>
    /// A simple category logger implementation for use with Microsoft Enterprise Library Logging Framework. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// This is useful if using Enterprise Library Logging Application Block with a multiple categories, without any other advance features, and only want to long simple string based messages to categories.
    /// </remarks>
    public sealed class EntLibSimpleCategoryLogger : EntLibLogger<MessageWithCategories>
    {        

        /// <summary>
        /// Initializes a new instance of the <see cref="EntLibSimpleCategoryLogger"/> class.
        /// </summary>
        /// <param name="logWriter">The log writer.</param>
        public EntLibSimpleCategoryLogger(LogWriter logWriter) : base(logWriter)
        {            
        }

        /// <summary>
        /// Returns true if the provided <see cref="T:Gnomesoft.Logging.LogSeverity" /> is currently in scope.
        /// </summary>
        /// <param name="severity">The severity to check if it is in scope.</param>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        /// <returns><c>true</c> if [is severity in scope] [the specified severity]; otherwise, <c>false</c>.</returns>
        /// <remarks>It is recommend that <paramref name="messageDelegate" /> not allowed to be null, but in some implementations this may not be easy or even possible.
        /// The implementation documentation should be referenced for more details.</remarks>
        public override bool IsSeverityInScope(LogSeverity severity, Func<MessageWithCategories> messageDelegate)
        {
            if (severity < LogSeverity.Off || severity > LogSeverity.All)
                return false;
            if (severity == LogSeverity.Off && !MyLogWriter.IsLoggingEnabled())
                return true;

            MessageWithCategories mwc = messageDelegate?.Invoke();

            LogSeverity categorySeverity;
            if (mwc != null && mwc.Categories.Length > 0)
            {
                foreach (var category in mwc.Categories)
                {
                    if (!CategoryMinimumSeverityMap.ContainsKey(category))
                        return false;
                    categorySeverity = CategoryMinimumSeverityMap[category];
                    if (severity == LogSeverity.Off && categorySeverity == LogSeverity.Off)
                        return true;
                    if (severity <= categorySeverity && categorySeverity >= LogSeverity.Off && severity != LogSeverity.Off)
                        return true;
                }
                return false;
            }

            if (string.IsNullOrWhiteSpace(DefaultCategory) || !CategoryMinimumSeverityMap.ContainsKey(DefaultCategory))
                return false;
            categorySeverity = CategoryMinimumSeverityMap[DefaultCategory];
            return severity <= categorySeverity && categorySeverity >= LogSeverity.Off && severity != LogSeverity.Off;
        }

        protected override void Write(LogSeverity severity, Func<MessageWithCategories> messageDelegate)
        {
            if (!IsSeverityInScope(severity, messageDelegate))
                return;

            string originalMessage = string.Empty;
            string[] originalCategories = new string[0];

            if (messageDelegate != null)
            {
                MessageWithCategories mwc = messageDelegate();

                if (!string.IsNullOrWhiteSpace(mwc?.Message))
                    originalMessage = mwc.Message;

                if (mwc != null)
                {
                    if (!string.IsNullOrWhiteSpace(mwc.Message))
                        originalMessage = mwc.Message;

                    originalCategories = mwc.Categories;
                }

            }
            
            if (string.IsNullOrEmpty(originalMessage))
                return;

            LogEntry entry = new LogEntry
            {
                Severity = LogSeverityToTraceEventTypeMap[severity],
                Message = originalMessage,
            };

            if (originalCategories.Length > 0)
                foreach (string categories in originalCategories)
                    entry.Categories.Add(categories);            
            else
                entry.Categories.Add(DefaultCategory);

            MyLogWriter.Write(entry);
        }

        protected override Func<MessageWithCategories> InjectExceptionIntoMessageDelegate(Func<MessageWithCategories> messageDelegate, Exception ex)
        {
            Func<MessageWithCategories> msg = () =>
            {
                Func<MessageWithCategories> localMessageDelegate = messageDelegate;
                Exception localException = ex;
                string originalMessage = string.Empty;
                string[] originalCategories = new string[0];

                if (localMessageDelegate != null)
                {
                    MessageWithCategories mwc = localMessageDelegate();

                    if (localException == null)
                        return mwc;

                    if (!string.IsNullOrWhiteSpace(mwc?.Message))
                        originalMessage = mwc.Message;

                    if (mwc != null)
                    {
                        if (!string.IsNullOrWhiteSpace(mwc.Message))
                            originalMessage = mwc.Message;

                        originalCategories = mwc.Categories;
                    }
                    
                }

                string mergedMessage = MergeMessageWithException(originalMessage, ex);

                return new MessageWithCategories(mergedMessage, originalCategories);
            };

            return msg;
        }
    }
}
