// <copyright file="LoggerService.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.CrossCutting
{
    using System.Diagnostics;

    /// <summary>
    /// Log with system diagnostic into the trace source.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.CrossCutting.ILoggerService" />
    public class LoggerService : ILoggerService
    {
        /// <summary>
        /// The trace source.
        /// </summary>
        private TraceSource trace = new TraceSource("CollectionneurRecettes.Addon");

        /// <inheritdoc />
        public void LogError(string message)
        {
            this.trace.TraceEvent(TraceEventType.Error, 2, message);
        }

        /// <inheritdoc />
        public void LogInformation(string log)
        {
            this.trace.TraceInformation(log);
        }

        /// <inheritdoc />
        public void LogVerbose(string log)
        {
            this.trace.TraceEvent(TraceEventType.Verbose, 1, log);
        }

        /// <inheritdoc />
        public void LogWarning(string log)
        {
            this.trace.TraceEvent(TraceEventType.Warning, 2, log);
        }
    }
}
