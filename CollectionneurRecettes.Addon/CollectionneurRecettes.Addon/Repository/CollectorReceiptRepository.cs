using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionneurRecettes.Addon.Entity.Data;
using System.Data;

namespace CollectionneurRecettes.Addon.Repository
{
    public class CollectorReceiptRepository : Interfaces.ICollectorReceiptRepository
    {
        private Interfaces.IH2Database h2Database;

        public CollectorReceiptRepository(Interfaces.IH2Database h2Database)
        {
            if (h2Database == null)
            {
                throw new ArgumentNullException("h2Database");
            }

            this.h2Database = h2Database;
        }

        public SelectMenuView SelectMenu(string connectionString, DateTime from, DateTime to)
        {
            var query = "select menu.day, recipes.title from menu, recipes " +
                        " where recipes.cidrecipe = menu.cidrecipe" +
                        " and menu.day >= '" + from.ToString("yyyy-MM-dd") +
                        "' and menu.day <= '" + to.ToString("yyyy-MM-dd") + "'";

            var selectResult = this.h2Database.ExecuteSelectQuery(connectionString, query);
            var result = new SelectMenuView();

            foreach (DataRow row in selectResult.Rows)
            {
                result.Rows.Add(new SelectMenuView.SelectMenuViewRow()
                {
                    Date = (DateTime)row["DAY"],
                    ReceiptTitle = (String)row["TITLE"]
                });
            }

            return result;
        }

        public void TryConnect(string connectionString)
        {
            this.h2Database.ExecuteSelectQuery(connectionString, "select cidrecipe from recipes where cidrecipe=1");
        }
    }
}
