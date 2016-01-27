// <copyright file="INetworkHelper.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Interfaces
{
    /// <summary>
    /// Helper for network
    /// </summary>
    public interface INetworkHelper
    {
        /// <summary>
        /// Determines whether [internet connection is available].
        /// </summary>
        /// <returns>True if internet is available otherwise false.</returns>
        bool IsInternetConnectionAvailable();
    }
}
