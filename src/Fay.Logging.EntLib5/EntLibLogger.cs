using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace Fay.Logging.EntLib5
{
    /// <summary>
    /// Abstract base class that provides common implementation of the <see cref="DelegateLogger{TMetadata}"/> for use with the Microsoft Enterprise Library Logging Framework.
    /// </summary>
    /// <typeparam name="TMetadata">The type of the metadata used for the underlying logger.</typeparam>
    public abstract class EntLibLogger<TMetadata> : DelegateLogger<TMetadata>
    {
        private readonly IDictionary<SourceLevels, LogSeverity> _sourceLevelToLogSeverityMap = new Dictionary<SourceLevels, LogSeverity>
        {
            {SourceLevels.Off, LogSeverity.Off},
            {SourceLevels.Critical, LogSeverity.Critical},
            {SourceLevels.Error, LogSeverity.Error},
            {SourceLevels.Warning, LogSeverity.Warning},
            {SourceLevels.Information, LogSeverity.Information},
            {SourceLevels.Verbose, LogSeverity.Verbose},
            {SourceLevels.All, LogSeverity.All},
            {SourceLevels.ActivityTracing, LogSeverity.All},
        };

        protected IDictionary<LogSeverity, TraceEventType> LogSeverityToTraceEventTypeMap { get; } = new Dictionary<LogSeverity, TraceEventType>
        {
            {LogSeverity.Critical, TraceEventType.Critical},
            {LogSeverity.Error, TraceEventType.Error},
            {LogSeverity.Warning, TraceEventType.Warning},
            {LogSeverity.Information, TraceEventType.Information},
            {LogSeverity.Verbose, TraceEventType.Verbose},
            {LogSeverity.All, TraceEventType.Verbose},
        };

        protected IDictionary<object, LogSeverity> CategoryMinimumSeverityMap { get; } = new Dictionary<object, LogSeverity>();
        
        protected LogWriter MyLogWriter { get; private set; }
        protected string DefaultCategory { get; private set; }

        protected EntLibLogger(LogWriter logWriter)
        {
            Contract.Requires<ArgumentNullException>(logWriter != null);
            Contract.Requires<ArgumentNullException>(logWriter.TraceSources != null);
            Contract.Ensures(MyLogWriter !=  null);

            MyLogWriter = logWriter;

            ICollection<LogSource> traceSourcesValues = logWriter.TraceSources.Values;
            if (traceSourcesValues.Count == 0)
                return;
            DefaultCategory = traceSourcesValues.First().Name;
            foreach (LogSource source in traceSourcesValues)
                CategoryMinimumSeverityMap.Add(source.Name, _sourceLevelToLogSeverityMap[source.Level]);
        }
        
        private bool _disposed;
        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            lock (this)
            {
                if (disposing)
                {
                    MyLogWriter.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}