

namespace CollectionneurRecettes.Addon.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface ISettingsManager
    {
        Entity.Settings LoadSettings();
        void SaveSettings(Entity.Settings settings);
        bool IsCollectionneurRecetteDbPathValid(string path);
    }
}
