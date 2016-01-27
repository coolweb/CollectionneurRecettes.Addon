// <copyright file="ICollectorReceiptManager.cs" company="No company">
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
    /// Manage receipts.
    /// </summary>
    public interface ICollectorReceiptManager
    {
        /// <summary>
        /// Retrieves the menu from the database between the period defined into the settings.
        /// </summary>
        /// <returns>The menu.</returns>
        Entity.Menu RetrieveMenu();

        /// <summary>
        /// Determines whether [is collector receipt application running].
        /// </summary>
        /// <returns>Indicates if the application is running</returns>
        bool IsCollectorReceiptAppRunning();

        /// <summary>
        /// Determines whether the application can synchronize menu with the online calendar.
        /// </summary>
        /// <returns>The reason if it can't sync.</returns>
        Entity.CanNotSyncReason CanSync();
    }
}
