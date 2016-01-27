// <copyright file="NetworkHelper.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>
namespace CollectionneurRecettes.Addon.Repository
{
    using System;
    using CollectionneurRecettes.Addon.CrossCutting;

    /// <summary>
    /// Helper for network
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.INetworkHelper" />
    public class NetworkHelper : Interfaces.INetworkHelper
    {
        /// <summary>
        /// The logger
        /// </summary>
        private ILoggerService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NetworkHelper"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">logger is null</exception>
        public NetworkHelper(ILoggerService logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }

        /// <inheritdoc />
        public bool IsInternetConnectionAvailable()
        {
            this.logger.LogVerbose("Check internet connection");
            return Marshals.ExternalsApi.IsConnectionAvailable();
        }
    }
}
