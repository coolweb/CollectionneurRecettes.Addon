using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Interfaces
{
    public interface ICollectorReceiptRepository
    {
        void TryConnect(string connectionString);
        Entity.Data.SelectMenuView SelectMenu(string connectionStrin, DateTime from, DateTime to);
    }
}
