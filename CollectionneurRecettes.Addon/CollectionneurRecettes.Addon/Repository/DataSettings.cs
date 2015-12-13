
namespace CollectionneurRecettes.Addon.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Entity;
    using System.IO;
    using System.Xml.Linq;

    public class DataSettings : Interfaces.IDataSettings
    {
        private string settingFilePath;

        public DataSettings()
        {
            this.settingFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CollectionneurRecettes.Addon\\Settings.xml");
        }

        public Settings ReadSettings()
        {
            if (File.Exists(this.settingFilePath))
            {
                var doc = XDocument.Load(this.settingFilePath);
                return new Settings()
                {
                    CollectionneurDabasePath = doc.Element("Settings").Attribute("CollectionneurRecettesDbPath").Value,
                    CalendarId = doc.Element("Settings").Attribute("CalendarId").Value,
                    DaysToSyncBefore = short.Parse(doc.Element("Settings").Attribute("DaysToSyncBefore").Value),
                    DaysToSyncAfter = short.Parse(doc.Element("Settings").Attribute("DaysToSyncAfter").Value)
                };
            } else
            {
                return new Settings();
            }
        }

        public void SaveSettings(Settings settings)
        {
            if(settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            if (!Directory.Exists(Path.GetDirectoryName(this.settingFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.settingFilePath));
            }

            var doc = new XDocument();
            doc.Add(
                new XElement("Settings", 
                    new XAttribute("CollectionneurRecettesDbPath", settings.CollectionneurDabasePath),
                    new XAttribute("CalendarId", settings.CalendarId),
                    new XAttribute("DaysToSyncBefore", settings.DaysToSyncBefore),
                    new XAttribute("DaysToSyncAfter", settings.DaysToSyncAfter)));
            doc.Save(this.settingFilePath);
        }
    }
}
