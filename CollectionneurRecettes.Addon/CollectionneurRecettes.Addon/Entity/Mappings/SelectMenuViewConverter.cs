using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Entity.Mappings
{
    public class SelectMenuViewConverter : ITypeConverter<Data.SelectMenuView, Menu>
    {
        public Menu Convert(ResolutionContext context)
        {
            var source = context.SourceValue as Data.SelectMenuView;

            var result = new Menu();
            var day = new Day();
            foreach (var row in source.Rows)
            {
                if (row.Date.Date != day.Date.Date)
                {
                    day = new Day();
                    day.Date = row.Date;
                    result.Days.Add(day);
                }

                var receipt = new Receipt();
                receipt.Name = row.ReceiptTitle;
                day.Receipts.Add(receipt);                
            }

            return result;
        }
    }
}
