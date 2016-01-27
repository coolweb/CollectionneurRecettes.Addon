// <copyright file="IGoogleManager.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Manage google account.
    /// </summary>
    public interface IGoogleManager
    {
        /// <summary>
        /// Determines whether [is account already configured].
        /// Check if data store file exists and secret file.
        /// </summary>
        /// <returns>[True] if account is configured, otherwise [False]</returns>
        bool IsAccountAlreadyConfigured();

        /// <summary>
        /// Determine if the secrets the file exists.
        /// </summary>
        /// <returns>[True] if secret file exists, otherwise [False]</returns>
        bool SecretFileExists();

        /// <summary>
        /// Clears the configured account by deleting the data store file if exist.
        /// </summary>
        void ClearConfiguredAccount();

        /// <summary>
        /// Configures the google account.
        /// </summary>
        /// <returns>Return true.</returns>
        Task<bool> ConfigureAccount();

        /// <summary>
        /// Loads the calendars from the google account.
        /// </summary>
        /// <returns>A list of calendars</returns>
        Task<IEnumerable<Entity.Calendar>> LoadCalendars();

        /// <summary>
        /// Creates the menus into the google calendar, if event already exists, it delete it and create it again.
        /// </summary>
        /// <param name="menu">The menu to create.</param>
        /// <param name="progress">The progress of the creation.</param>
        /// <returns>The number of receipts created/updated</returns>
        Task<int> CreateMenus(Entity.Menu menu, IProgress<Entity.ProgressCreateMenus> progress);
    }
}
