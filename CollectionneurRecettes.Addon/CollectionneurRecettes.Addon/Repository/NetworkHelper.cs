
namespace CollectionneurRecettes.Addon.Repository
{
    /// <summary>
    /// <inheritedDoc />
    /// </summary>
    public class NetworkHelper : Interfaces.INetworkHelper
    {
        /// <inheritedDoc />
        public bool IsInternetConnectionAvailable()
        {
            return Marshals.ExternalsApi.IsConnectionAvailable();
        }
    }
}
