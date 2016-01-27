// <copyright file="ICollectorReceiptRepository.cs" company="No company">
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
    /// Manage receipts from receipt application. 
    /// </summary>
    public interface ICollectorReceiptRepository
    {
        /// <summary>
        /// Tries a connection to the database by executing a query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        void TryConnect(string connectionString);

        /// <summary>
        /// Selects the menu.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <returns>The result of the select.</returns>
        Entity.Data.SelectMenuView SelectMenu(string connectionString, DateTime from, DateTime to);
    }
}
