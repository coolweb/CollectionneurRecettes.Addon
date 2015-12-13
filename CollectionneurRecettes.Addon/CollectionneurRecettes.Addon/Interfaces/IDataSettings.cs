

namespace CollectionneurRecettes.Addon.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IDataSettings
    {
        Entity.Settings ReadSettings();
        void SaveSettings(Entity.Settings settings);
    }
}
