using System;
using System.Collections.Generic;
using System.Data;
using System.Data.H2;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Repository
{
    internal class H2Database : Interfaces.IH2Database
    {
        public DataTable ExecuteSelectQuery(string connectionString, string query)
        {
            using (System.Data.H2.H2Connection connection = new System.Data.H2.H2Connection(connectionString, "sa", string.Empty))
            {
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
