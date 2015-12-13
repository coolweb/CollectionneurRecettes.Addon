using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Entity.Data
{
    public class SelectMenuView
    {
        public SelectMenuView()
        {
            this.Rows = new List<SelectMenuViewRow>();
        }

        public class SelectMenuViewRow
        {
            public DateTime Date { get; set; }
            public string ReceiptTitle { get; set; }
        }

        public List<SelectMenuViewRow> Rows { get; set; }
    }
}
