using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fay.Logging
{
    /// <summary>
    /// A null logger implementation of the <see cref="IDelegateLogger{TMetadata}"/> interface. All log requests are ignored, and all severities are out of scope. 
    /// </summary>
    ///  <typeparam name="TMetadata">The type of the metadata used for the underlying logger.</typeparam>
    public class NullLogger<TMetadata> : IDelegateLogger<TMetadata>
    {
        /// <summary>
        /// Does nothing, as nothing needs to be disposed.
        /// </summary>
        public void Dispose()
        {   
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="messageDelegate">Value is ignored.</param>
        public void Critical(Func<TMetadata> messageDelegate)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="messageDelegate">Value is ignored.</param>
        public void Error(Func<TMetadata> messageDelegate)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="messageDelegate">Value is ignored.</param>
        public void Warning(Func<TMetadata> messageDelegate)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="messageDelegate">Value is ignored.</param>
        public void Information(Func<TMetadata> messageDelegate)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="messageDelegate">Value is ignored.</param>
        public void Verbose(Func<TMetadata> messageDelegate)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="messageDelegate">Value is ignored.</param>
        /// <param name="ex">Value is ignored.</param>
        public void CriticalException(Func<TMetadata> messageDelegate, Exception ex)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="messageDelegate">Value is ignored.</param>
        /// <param name="ex">Value is ignored.</param>
        public void ErrorException(Func<TMetadata> messageDelegate, Exception ex)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="severity">Value is ignored.</param>
        /// <param name="messageDelegate">Value is ignored.</param>
        /// <param name="ex">Value is ignored.</param>
        public void Exception(LogSeverity severity, Func<TMetadata> messageDelegate, Exception ex)
        {
        }

        /// <summary>
        /// Ignores all values and does nothing.
        /// </summary>
        /// <param name="severity">Value is ignored.</param>
        /// <param name="messageDelegate">Value is ignored.</param>
        public void Message(LogSeverity severity, Func<TMetadata> messageDelegate)
        {
        }

        /// <summary>
        /// Always returns false.
        /// </summary>
        /// <param name="severity">Value is ignored.</param>
        /// <param name="messageDelegate">Value is ignored.</param>
        /// <returns><c>false</c></returns>
        public bool IsSeverityInScope(LogSeverity severity, Func<TMetadata> messageDelegate)
        {
            return false;
        }
    }
}
