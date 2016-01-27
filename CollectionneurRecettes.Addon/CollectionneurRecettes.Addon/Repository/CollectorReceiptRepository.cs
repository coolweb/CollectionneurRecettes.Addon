// <copyright file="CollectorReceiptRepository.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Repository
{
    using System;
    using System.Data;
    using CollectionneurRecettes.Addon.Entity.Data;

    /// <summary>
    /// Manage receipts from receipt application. 
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.ICollectorReceiptRepository" />
    public class CollectorReceiptRepository : Interfaces.ICollectorReceiptRepository
    {
        /// <summary>
        /// The h2 database
        /// </summary>
        private Interfaces.IH2Database database;

        /// <summary>
        /// The logger
        /// </summary>
        private CrossCutting.ILoggerService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorReceiptRepository"/> class.
        /// </summary>
        /// <param name="database">The h2 database.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// h2Database
        /// or
        /// logger
        /// </exception>
        public CollectorReceiptRepository(Interfaces.IH2Database database, CrossCutting.ILoggerService logger)
        {
            if (database == null)
            {
                throw new ArgumentNullException("h2Database");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.database = database;
            this.logger = logger;
        }

        /// <summary>
        /// Selects the menu.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="from">From date.</param>
        /// <param name="to">To date.</param>
        /// <returns>The result of the select.</returns>
        public SelectMenuView SelectMenu(string connectionString, DateTime from, DateTime to)
        {
            var query = "select menu.day, recipes.title from menu, recipes " +
                        " where recipes.cidrecipe = menu.cidrecipe" +
                        " and menu.day >= '" + from.ToString("yyyy-MM-dd") +
                        "' and menu.day <= '" + to.ToString("yyyy-MM-dd") + "'" +
                        "order by menu.day asc";

            this.logger.LogVerbose(string.Format("Execute query {0}", query));
            var selectResult = this.database.ExecuteSelectQuery(connectionString, query);
            var result = new SelectMenuView();

            foreach (DataRow row in selectResult.Rows)
            {
                result.Rows.Add(new SelectMenuView.SelectMenuViewRow()
                {
                    Date = (DateTime)row["DAY"],
                    ReceiptTitle = (string)row["TITLE"]
                });
            }

            this.logger.LogInformation(string.Format("{0} receipts loaded from database between {1} and {2}", result.Rows.Count, from, to));
            return result;
        }

        /// <summary>
        /// Tries a connection to the database by executing a query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public void TryConnect(string connectionString)
        {
            var query = "select cidrecipe from recipes where cidrecipe=1";

            this.logger.LogVerbose(string.Format("Execute query {0}", query));
            this.database.ExecuteSelectQuery(connectionString, query);
        }
    }
}
