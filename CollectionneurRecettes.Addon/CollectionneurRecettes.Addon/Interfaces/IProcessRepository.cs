// <copyright file="IProcessRepository.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Interfaces
{
    using System.Diagnostics;

    /// <summary>
    /// Process repository.
    /// </summary>
    public interface IProcessRepository
    {
        /// <summary>
        /// Search for a running process.
        /// </summary>
        /// <param name="name">The name of the process to search.</param>
        /// <returns>The found process or null</returns>
        Process RunningProcess(string name);
    }
}
