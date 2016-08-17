// <copyright file="ILoggerService.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.CrossCutting
{
    /// <summary>
    /// The logger service interface.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Logs the information.
        /// </summary>
        /// <param name="log">The log.</param>
        void LogInformation(string log);

        /// <summary>
        /// Logs the verbose.
        /// </summary>
        /// <param name="log">The log.</param>
        void LogVerbose(string log);

        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="log">The log.</param>
        void LogWarning(string log);

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="message">The message.</param>
        void LogError(string message);
    }
}
