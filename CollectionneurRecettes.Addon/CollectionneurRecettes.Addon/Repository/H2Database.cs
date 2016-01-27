// <copyright file="H2Database.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Repository
{
    using System;
    using System.Data;
    using System.Data.H2;
    using CollectionneurRecettes.Addon.CrossCutting;

    /// <summary>
    /// Manage queries to the collector application database.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.IH2Database" />
    internal class H2Database : Interfaces.IH2Database
    {
        /// <summary>
        /// The logger
        /// </summary>
        private ILoggerService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="H2Database"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">logger is null</exception>
        public H2Database(ILoggerService logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }

        /// <summary>
        /// Executes the select query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="query">The query.</param>
        /// <returns>A data table containing the result of the query</returns>
        public DataTable ExecuteSelectQuery(string connectionString, string query)
        {
            using (System.Data.H2.H2Connection connection = new System.Data.H2.H2Connection(connectionString, "sa", string.Empty))
            {
                this.logger.LogVerbose(string.Format("Execute query {0}", query));
                connection.Open();

                var com = new H2Command(query, connection);
                var result = com.ExecuteReader();
                var tableResult = new DataTable();

                for (int i = 0; i < result.FieldCount; i++)
                {
                    tableResult.Columns.Add(result.GetName(i), result.GetFieldType(i));
                }

                while (result.Read())
                {
                    var row = tableResult.NewRow();

                    for (int i = 0; i < result.FieldCount; i++)
                    {
                        row[i] = result.GetValue(i);
                    }

                    tableResult.Rows.Add(row);
                }

                return tableResult;
            }
        }
    }
}
