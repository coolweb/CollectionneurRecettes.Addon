using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Entity
{
    public class Day
    {
        public Day()
        {
            this.Receipts = new List<Receipt>();
        }

        public DateTime Date { get; set; }
        public List<Receipt> Receipts { get; set; }
    }
}
