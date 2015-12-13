

namespace CollectionneurRecettes.Addon.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Entity;

    public class SettingsManager : Interfaces.ISettingsManager
    {
        private Interfaces.IDataSettings dataSettings;
        private Interfaces.ICollectorReceiptRepository collectorReceiptRepository;

        public SettingsManager(Interfaces.IDataSettings dataSettings, Interfaces.ICollectorReceiptRepository collectorReceiptRepository)
        {
            if (dataSettings == null)
            {
                throw new ArgumentNullException("dataSettings");
            }

            if (collectorReceiptRepository == null)
            {
                throw new ArgumentNullException("collectorReceiptRepository");
            }

            this.collectorReceiptRepository = collectorReceiptRepository;
            this.dataSettings = dataSettings;
        }

        public bool IsCollectionneurRecetteDbPathValid(string path)
        {
            try
            {
                this.collectorReceiptRepository.TryConnect("jdbc:h2:" + path.Replace(".h2.db", string.Empty));
                return true;
            }
            catch(System.Data.H2.H2Exception)
            {
                return false;
            }
        }

        public Settings LoadSettings()
        {
            return this.dataSettings.ReadSettings();
        }

        public void SaveSettings(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            this.dataSettings.SaveSettings(settings);
        }
    }
}
