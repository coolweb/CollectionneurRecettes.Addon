using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Entity
{
    public class Menu
    {
        public Menu()
        {
            this.Days = new List<Day>();
        }

        public List<Day> Days { get; set; }
    }
}
