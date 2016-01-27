// <copyright file="IH2Database.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Interfaces
{
    using System.Data;

    /// <summary>
    /// Manage queries to the collector application database.
    /// </summary>
    public interface IH2Database
    {
        /// <summary>
        /// Executes the select query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="query">The query.</param>
        /// <returns>A data table containing the result of the query</returns>
        DataTable ExecuteSelectQuery(string connectionString, string query);
    }
}
