using System;
using System.Text;

namespace Fay.Logging
{
    /// <summary>
    /// Abstract base class that provides an implementation that redirects all the convenience methods to the protected abstract Write method.
    /// </summary>
    /// <typeparam name="TMetadata">The type of the metadata used for the underlying logger.</typeparam>
    public abstract class DelegateLogger<TMetadata> : IDelegateLogger<TMetadata>
    {

        #region IDelegateLogger Convenience Method Redirects
        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> at the <see cref="F:Fay.Logging.LogSeverity.Critical" /> severity.
        /// </summary>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        public virtual void Critical(Func<TMetadata> messageDelegate)
        {
            Message(LogSeverity.Critical, messageDelegate);
        }

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> at the <see cref="F:Fay.Logging.LogSeverity.Error" /> severity.
        /// </summary>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        public virtual void Error(Func<TMetadata> messageDelegate)
        {
            Message(LogSeverity.Error, messageDelegate);
        }

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> at the <see cref="F:Fay.Logging.LogSeverity.Warning" /> severity.
        /// </summary>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        public virtual void Warning(Func<TMetadata> messageDelegate)
        {
            Message(LogSeverity.Warning, messageDelegate);
        }

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> at the <see cref="F:Fay.Logging.LogSeverity.Information" /> severity.
        /// </summary>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        public virtual void Information(Func<TMetadata> messageDelegate)
        {
            Message(LogSeverity.Information, messageDelegate);
        }
        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> at the <see cref="F:Fay.Logging.LogSeverity.Verbose" /> severity.
        /// </summary>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        public virtual void Verbose(Func<TMetadata> messageDelegate)
        {
            Message(LogSeverity.Verbose, messageDelegate);
        }

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> along with the provided exception at the <see cref="F:Fay.Logging.LogSeverity.Critical" /> severity.
        /// </summary>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        /// <param name="ex">The exception to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the exception will never be called.
        /// If the exception is null it will be ignored.
        /// If the exception is an <see cref="T:System.AggregateException" /> it will be flattened before being used.</param>
        public virtual void CriticalException(Func<TMetadata> messageDelegate, Exception ex)
        {
            Exception(LogSeverity.Critical, messageDelegate, ex);
        }

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> along with the provided exception at the <see cref="F:Fay.Logging.LogSeverity.Error" /> severity.
        /// </summary>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        /// <param name="ex">The exception to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the exception will never be called.
        /// If the exception is null it will be ignored.
        /// If the exception is an <see cref="T:System.AggregateException" /> it will be flattened before being used.</param>
        public virtual void ErrorException(Func<TMetadata> messageDelegate, Exception ex)
        {
            Exception(LogSeverity.Error, messageDelegate, ex);
        }

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> along with the provided exception at the specified <see cref="T:Fay.Logging.LogSeverity" />.
        /// </summary>
        /// <param name="severity">The <see cref="T:Fay.Logging.LogSeverity" /> to use.</param>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        /// <param name="ex">The exception to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the exception will never be called.
        /// If the exception is null it will be ignored.
        /// If the exception is an <see cref="T:System.AggregateException" /> it will be flattened before being used.</param>
        public virtual void Exception(LogSeverity severity, Func<TMetadata> messageDelegate, Exception ex)
        {
            if (messageDelegate == null && ex == null)
                return;

            Message(severity, InjectExceptionIntoMessageDelegate(messageDelegate, ex));
        }

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate" /> at the specified <see cref="T:Fay.Logging.LogSeverity" />.
        /// </summary>
        /// <param name="severity">The <see cref="T:Fay.Logging.LogSeverity" /> to use.</param>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        public virtual void Message(LogSeverity severity, Func<TMetadata> messageDelegate)
        {
            Write(severity, messageDelegate);
        }
        #endregion

        #region IDelegateLogger Abstract Methods                
        /// <summary>
        /// Returns true if the provided <see cref="T:Fay.Logging.LogSeverity" /> is currently in scope.
        /// </summary>
        /// <param name="severity">The severity to check if it is in scope.</param>
        /// <param name="messageDelegate">The delegate to be use to obtain the message to log if the severity is within scope.
        /// If the severity is outside of the scope to be logged then the delegate will never be called.
        /// If the delegate is null it will be ignored.</param>
        /// <returns><c>true</c> if [is severity in scope] [the specified severity]; otherwise, <c>false</c>.</returns>
        /// <remarks>It is recommend that <paramref name="messageDelegate" /> not allowed to be null, but in some implementations this may not be easy or even possible.
        /// The implementation documentation should be referenced for more details.</remarks>
        public abstract bool IsSeverityInScope(LogSeverity severity, Func<TMetadata> messageDelegate);
        /// <summary>
        /// Implementation specific method that actually writes to underlying logger.
        /// </summary>
        /// <param name="severity">
        /// The <see cref="LogSeverity"/> to use.
        /// </param>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>   
        protected abstract void Write(LogSeverity severity, Func<TMetadata> messageDelegate);

        /// <summary>
        /// Used to inject, replace, or wrap the provided <paramref name="messageDelegate"/> with the provided <paramref name="ex"/>.
        /// </summary>
        /// <param name="messageDelegate">The message delegate to inject, replace, or wrap.</param>
        /// <param name="ex">The exception to inject, replace, or wrap.</param>
        /// <returns>
        /// A delegate that will return the results of the <paramref name="messageDelegate"/> and <paramref name="ex"/>.
        /// </returns>
        protected abstract Func<TMetadata> InjectExceptionIntoMessageDelegate(Func<TMetadata> messageDelegate, Exception ex);
        #endregion

        #region Dispose Pattern Implementation        
        /// <summary>
        /// Implemented following Microsoft's Dispose Pattern, therefore if disposing of resources is required please override the protected Dispoise(bool).
        /// See MSDN Documentation for more details: https://msdn.microsoft.com/library/b1yfkh5e.aspx
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Placeholder method to use if needed to dispose of resources.  
        /// Default implementation does nothing. 
        /// If overridden the implementation should follow Microsoft's Dispose Pattern, the implemented <see cref="IDisposable.Dispose"/> already does and calls this method with a true.
        /// See MSDN Documentation for more details: https://msdn.microsoft.com/library/b1yfkh5e.aspx
        /// </summary>
        /// <param name="disposing">
        /// true if method was invoked from the <see cref="IDisposable.Dispose"/> implementation; other method was invoked from the finalizer.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }
        #endregion

        public const string BeginExceptionDetailsText = "[--- Begin Exception Details ---]";
        public const string EndExceptionDetailsText = "[--- End Exception Details ---]";

        /// <summary>
        /// Merges the message with exception by having the <paramref name="message"/> be the first line followed by the results of  
        /// <see cref="System.Exception.ToString()"/> being placed within a <c>begin</c> and <c>end</c> block to set it appart. 
        /// </summary>
        /// <param name="message">
        /// The message to merge. 
        /// If null or empty will be ignored, if it is only whitespace it will be used. 
        /// </param>
        /// <param name="ex">
        /// The ex to merge.
        /// If null will return the provided <paramref name="message"/>.
        /// </param>
        /// <returns>Newly merged message</returns>
        protected virtual string MergeMessageWithException(string message, Exception ex)
        {
            StringBuilder builder = new StringBuilder();

            if (ex == null)
                return message;            

            if (!string.IsNullOrEmpty(message))
                builder.Append(message);

            builder.Append(Environment.NewLine);
            builder.Append(BeginExceptionDetailsText);
            builder.Append(Environment.NewLine);
            AggregateException aggregateException = ex as AggregateException;
            builder.Append(aggregateException?.Flatten() ?? ex);
            builder.Append(Environment.NewLine);
            builder.Append(EndExceptionDetailsText);

            return builder.ToString();
        }
    }
}
