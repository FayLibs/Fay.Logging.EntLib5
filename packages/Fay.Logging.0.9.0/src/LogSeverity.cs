namespace Fay.Logging
{
    /// <summary>
    /// Specifies the log levels available for message filtering. With Critical being the highest severity level and Verbose the lowest severity level.
    /// </summary>
    public enum LogSeverity : short
    {
        /// <summary>
        /// This will disable logging and not allow any messages through to the log.
        /// </summary>
        Off = 0,

        /// <summary>
        /// This level will allow only Critical messages through to the log.
        /// </summary>
        Critical = 1,

        /// <summary>
        /// This level will allow only Error and higher messages through to the log.
        /// </summary>
        Error = 2,

        /// <summary>
        /// This level will allow only Warning and higher messages through to the log.
        /// </summary>
        Warning = 4,

        /// <summary>
        /// This level will allow only Information and higher messages through to the log.
        /// </summary>
        Information = 8,

        /// <summary>
        /// This level will allow only Verbose and higher messages through to the log. (This currently is equivalent to All.)
        /// </summary>
        Verbose = 16,

        /// <summary>
        /// This level will allows all messages through to the log.
        /// </summary>
        All = short.MaxValue
    }
}