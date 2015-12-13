using System;
using System.Collections.Generic;
using System.Data;
using System.Data.H2;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Interfaces
{
    public interface IH2Database
    {
        DataTable ExecuteSelectQuery(string connectionString, string query);
    }
}
