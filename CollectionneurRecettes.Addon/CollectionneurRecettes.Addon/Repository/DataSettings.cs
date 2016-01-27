// <copyright file="DataSettings.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Repository
{
    using System;
    using System.IO;
    using System.Xml.Linq;
    using Entity;

    /// <summary>
    /// The settings data layer.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.IDataSettings" />
    public class DataSettings : Interfaces.IDataSettings
    {
        /// <summary>
        /// The logger
        /// </summary>
        private CrossCutting.ILoggerService logger;

        /// <summary>
        /// The setting file path
        /// </summary>
        private string settingFilePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSettings"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">logger is null</exception>
        public DataSettings(CrossCutting.ILoggerService logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
            this.settingFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CollectionneurRecettes.Addon\\Settings.xml");
        }

        /// <summary>
        /// Reads the settings.
        /// </summary>
        /// <returns>The settings loaded.</returns>
        public Settings ReadSettings()
        {
            if (File.Exists(this.settingFilePath))
            {
                this.logger.LogInformation(string.Format("File {0} exists, load it"));

                var doc = XDocument.Load(this.settingFilePath);
                return new Settings()
                {
                    CollectionneurDabasePath = doc.Element("Settings").Attribute("CollectionneurRecettesDbPath").Value,
                    CalendarId = doc.Element("Settings").Attribute("CalendarId").Value,
                    DaysToSyncBefore = short.Parse(doc.Element("Settings").Attribute("DaysToSyncBefore").Value),
                    DaysToSyncAfter = short.Parse(doc.Element("Settings").Attribute("DaysToSyncAfter").Value)
                };
            }
            else
            {
                this.logger.LogWarning(string.Format("File {0} doesn't exist, return new setting object", this.settingFilePath));
                return new Settings();
            }
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">settings is null</exception>
        public void SaveSettings(Settings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            var directory = Path.GetDirectoryName(this.settingFilePath);
            if (!Directory.Exists(directory))
            {
                this.logger.LogVerbose(string.Format("Create directory {0}", directory));
                Directory.CreateDirectory(Path.GetDirectoryName(this.settingFilePath));
            }

            var doc = new XDocument();
            doc.Add(
                new XElement(
                    "Settings",
                    new XAttribute("CollectionneurRecettesDbPath", settings.CollectionneurDabasePath),
                    new XAttribute("CalendarId", settings.CalendarId),
                    new XAttribute("DaysToSyncBefore", settings.DaysToSyncBefore),
                    new XAttribute("DaysToSyncAfter", settings.DaysToSyncAfter)));
            doc.Save(this.settingFilePath);

            this.logger.LogInformation(string.Format("Settings save to {0}", this.settingFilePath));
        }
    }
}
