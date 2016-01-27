// <copyright file="ProcessRepository.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Repository
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using CollectionneurRecettes.Addon.CrossCutting;

    /// <summary>
    /// Process repository.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.IProcessRepository" />
    public class ProcessRepository : Interfaces.IProcessRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private ILoggerService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ProcessRepository(ILoggerService logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }

        /// <summary>
        /// Search for a running process.
        /// </summary>
        /// <param name="name">The name of the process to search.</param>
        /// <returns>The found process or null</returns>
        public Process RunningProcess(string name)
        {
            this.logger.LogVerbose(string.Format("Search running process with name {0}", name));
            var foundProcesses = Process.GetProcessesByName(name).FirstOrDefault();

            return foundProcesses;
        }
    }
}
