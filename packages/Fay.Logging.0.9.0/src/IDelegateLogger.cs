using System;

namespace Fay.Logging
{
    /// <summary>
    /// Interface to describe a message delegate based logger.
    /// </summary>
    /// <typeparam name="TMetadata">The type of the metadata used for the underlying logger.</typeparam>
    public interface IDelegateLogger<in TMetadata> : IDisposable
    {
        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> at the <see cref="LogSeverity.Critical"/> severity.
        /// </summary>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>        
        void Critical(Func<TMetadata> messageDelegate);
        
        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> at the <see cref="LogSeverity.Error"/> severity.
        /// </summary>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>        
        void Error(Func<TMetadata> messageDelegate);

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> at the <see cref="LogSeverity.Warning"/> severity.
        /// </summary>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>        
        void Warning(Func<TMetadata> messageDelegate);
        
        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> at the <see cref="LogSeverity.Information"/> severity.
        /// </summary>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>        
        void Information(Func<TMetadata> messageDelegate);

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> at the <see cref="LogSeverity.Verbose"/> severity.
        /// </summary>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>        
        void Verbose(Func<TMetadata> messageDelegate);

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> along with the provided exception at the <see cref="LogSeverity.Critical"/> severity.
        /// </summary>      
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>       
        /// <param name="ex">
        /// The exception to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the exception will never be called. 
        /// If the exception is null it will be ignored.
        /// If the exception is an <see cref="AggregateException"/> it will be flattened before being used.
        /// </param>
        void CriticalException(Func<TMetadata> messageDelegate, Exception ex);

        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> along with the provided exception at the <see cref="LogSeverity.Error"/> severity.
        /// </summary>      
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>       
        /// <param name="ex">
        /// The exception to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the exception will never be called. 
        /// If the exception is null it will be ignored.
        /// If the exception is an <see cref="AggregateException"/> it will be flattened before being used.
        /// </param>
        void ErrorException(Func<TMetadata> messageDelegate, Exception ex);
        
        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> along with the provided exception at the specified <see cref="LogSeverity"/>.
        /// </summary>
        /// <param name="severity">
        /// The <see cref="LogSeverity"/> to use.
        /// </param>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>       
        /// <param name="ex">
        /// The exception to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the exception will never be called. 
        /// If the exception is null it will be ignored.
        /// If the exception is an <see cref="AggregateException"/> it will be flattened before being used.
        /// </param>
        void Exception(LogSeverity severity, Func<TMetadata> messageDelegate, Exception ex);
        
        /// <summary>
        /// Attempt to log the result the <paramref name="messageDelegate"/> at the specified <see cref="LogSeverity"/>.
        /// </summary>
        /// <param name="severity">
        /// The <see cref="LogSeverity"/> to use.
        /// </param>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param>       
        void Message(LogSeverity severity, Func<TMetadata> messageDelegate);
        
        /// <summary>
        /// Returns true if the provided <see cref="LogSeverity"/> is currently in scope.
        /// </summary>
        /// <param name="severity">The severity to check if it is in scope.</param>
        /// <param name="messageDelegate">
        /// The delegate to be use to obtain the message to log if the severity is within scope. 
        /// If the severity is outside of the scope to be logged then the delegate will never be called. 
        /// If the delegate is null it will be ignored.
        /// </param> 
        /// <remarks>
        /// It is recommend that <paramref name="messageDelegate"/> not allowed to be null, but in some implementations this may not be easy or even possible.
        /// The implementation documentation should be referenced for more details.
        /// </remarks>
        bool IsSeverityInScope(LogSeverity severity, Func<TMetadata> messageDelegate);        
    }
}