// <copyright file="SettingsManager.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Business
{
    using System;
    using CollectionneurRecettes.Addon.CrossCutting;
    using Entity;

    /// <summary>
    /// The manager for the settings of the application.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.ISettingsManager" />
    public class SettingsManager : Interfaces.ISettingsManager
    {
        /// <summary>
        /// The data settings
        /// </summary>
        private Interfaces.IDataSettings dataSettings;

        /// <summary>
        /// The collector receipt repository
        /// </summary>
        private Interfaces.ICollectorReceiptRepository collectorReceiptRepository;

        /// <summary>
        /// The logger
        /// </summary>
        private ILoggerService logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        /// <param name="dataSettings">The data settings.</param>
        /// <param name="collectorReceiptRepository">The collector receipt repository.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">
        /// dataSettings
        /// or
        /// collectorReceiptRepository
        /// or
        /// logger
        /// </exception>
        public SettingsManager(
            Interfaces.IDataSettings dataSettings,
            Interfaces.ICollectorReceiptRepository collectorReceiptRepository,
            ILoggerService logger)
        {
            if (dataSettings == null)
            {
                throw new ArgumentNullException("dataSettings");
            }

            if (collectorReceiptRepository == null)
            {
                throw new ArgumentNullException("collectorReceiptRepository");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.collectorReceiptRepository = collectorReceiptRepository;
            this.dataSettings = dataSettings;
            this.logger = logger;
        }

        /// <summary>
        /// Determines whether database path is valid.
        /// </summary>
        /// <param name="path">The path of the database.</param>
        /// <returns>True if path is valid, otherwise false.</returns>
        public bool IsCollectionneurRecetteDbPathValid(string path)
        {
            try
            {
                this.collectorReceiptRepository.TryConnect("jdbc:h2:" + path.Replace(".h2.db", string.Empty));
                this.logger.LogInformation("Connect to database successfull");
                return true;
            }
            catch (System.Data.H2.H2Exception)
            {
                this.logger.LogInformation("Connect to database failed");
                return false;
            }
        }

        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <returns>A setting of object.</returns>
        public Settings LoadSettings()
        {
            this.logger.LogInformation("Load settings of the application");
            return this.dataSettings.ReadSettings();
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

            this.logger.LogInformation("Save the settings");
            this.dataSettings.SaveSettings(settings);
        }
    }
}