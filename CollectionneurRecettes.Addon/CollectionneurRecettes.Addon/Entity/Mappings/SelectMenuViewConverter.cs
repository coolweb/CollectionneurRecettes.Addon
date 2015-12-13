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
            foreach (var row in source.Rows)
            {
                var day = new Day();
                day.Date = row.Date;

                var receipt = new Receipt();
                receipt.Name = row.ReceiptTitle;
                day.Receipts.Add(receipt);

                result.Days.Add(day);
            }

            return result;
        }
    }
}
