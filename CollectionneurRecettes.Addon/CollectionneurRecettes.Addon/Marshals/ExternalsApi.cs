// <copyright file="ExternalsApi.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Marshals
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// External win 32 Api.
    /// </summary>
    public static class ExternalsApi
    {
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int connDescription, int ReservedValue);

        /// <summary>
        /// Check if a connection to the Internet can be established
        /// </summary>
        /// <returns><c>True</c> if a connection can be etablished otherwise <c>False</c></returns>
        public static bool IsConnectionAvailable()
        {
            int connDesc;
            return InternetGetConnectedState(out connDesc, 0);
        }
    }
}
