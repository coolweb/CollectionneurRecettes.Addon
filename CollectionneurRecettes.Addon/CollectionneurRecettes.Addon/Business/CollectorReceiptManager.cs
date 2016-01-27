// <copyright file="CollectorReceiptManager.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Business
{
    using System;
    using System.Diagnostics;
    using AutoMapper;
    using CollectionneurRecettes.Addon.CrossCutting;
    using CollectionneurRecettes.Addon.Entity;

    /// <summary>
    /// Manage receipts.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.ICollectorReceiptManager" />
    public class CollectorReceiptManager : Interfaces.ICollectorReceiptManager
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private ILoggerService logger;

        /// <summary>
        /// The database connection string.
        /// </summary>
        private string databaseConnectionString;

        /// <summary>
        /// The settings of the application.
        /// </summary>
        private Entity.Settings settings;

        /// <summary>
        /// The settings manager.
        /// </summary>
        private Interfaces.ISettingsManager settingsManager;

        /// <summary>
        /// The receipt repository.
        /// </summary>
        private Interfaces.ICollectorReceiptRepository receiptRepository;

        /// <summary>
        /// The process repository.
        /// </summary>
        private Interfaces.IProcessRepository processRepository;

        /// <summary>
        /// The collector receipt process
        /// </summary>
        private Process collectorReceiptProcess;

        /// <summary>
        /// The network helper
        /// </summary>
        private Interfaces.INetworkHelper networkHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectorReceiptManager" /> class.
        /// </summary>
        /// <param name="receiptRepository">The receipt repository.</param>
        /// <param name="processRepository">The process repository.</param>
        /// <param name="settingsManager">The settings manager.</param>
        /// <param name="networkHelper">The network helper.</param>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">settingsManager
        /// or
        /// receiptRepository
        /// or
        /// processRepository
        /// or
        /// networkHelper
        /// or
        /// logger</exception>
        public CollectorReceiptManager(
            Interfaces.ICollectorReceiptRepository receiptRepository, 
            Interfaces.IProcessRepository processRepository,
            Interfaces.ISettingsManager settingsManager,
            Interfaces.INetworkHelper networkHelper,
            ILoggerService logger)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            if (receiptRepository == null)
            {
                throw new ArgumentNullException("receiptRepository");
            }

            if (processRepository == null)
            {
                throw new ArgumentNullException("processRepository");
            }

            if (networkHelper == null)
            {
                throw new ArgumentNullException("networkHelper");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.receiptRepository = receiptRepository;
            this.processRepository = processRepository;
            this.settingsManager = settingsManager;
            this.settings = this.settingsManager.LoadSettings();
            this.networkHelper = networkHelper;
            this.logger = logger;

            this.databaseConnectionString = "jdbc:h2:" + this.settings.CollectionneurDabasePath.Replace(".h2.db", string.Empty);
        }

        /// <summary>
        /// Retrieves the menu from the database between the period defined into the settings.
        /// </summary>
        /// <returns>The menu.</returns>
        public Menu RetrieveMenu()
        {
            this.logger.LogInformation("Retrieve menu from the database");

            var data = this.receiptRepository.SelectMenu(
                this.databaseConnectionString,
                DateTime.Now.AddDays(this.settings.DaysToSyncBefore * -1),
                DateTime.Now.AddDays(this.settings.DaysToSyncAfter));

            return Mapper.Map<Menu>(data);
        }

        /// <summary>
        /// Determines whether [is collector receipt application running].
        /// </summary>
        /// <returns>Indicates if the application is running</returns>
        public bool IsCollectorReceiptAppRunning()
        {
            if (this.collectorReceiptProcess == null)
            {
                this.logger.LogVerbose("The collector app was not running before");
                this.collectorReceiptProcess = this.processRepository.RunningProcess("collectionneurderecettes");

                if (this.collectorReceiptProcess != null)
                {
                    this.logger.LogVerbose("The collector app is detected, attach to exit event");
                    this.collectorReceiptProcess.EnableRaisingEvents = true;
                    this.collectorReceiptProcess.Exited += (sender, args) => 
                    {
                        this.logger.LogVerbose("The collector app exited");
                        this.collectorReceiptProcess = null;
                    };
                }
            }

            var isRunning = this.collectorReceiptProcess != null && !this.collectorReceiptProcess.HasExited;
            this.logger.LogInformation(string.Format("Is the collector app running? {0}", isRunning));

            return isRunning;
        }

        /// <summary>
        /// Determines whether the application can synchronize menu with the online calendar.
        /// </summary>
        /// <returns>The reason if it can't sync.</returns>
        public CanNotSyncReason CanSync()
        {
            var reason = CanNotSyncReason.None;

            // test settings
            var settings = this.settingsManager.LoadSettings();

            if (string.IsNullOrEmpty(settings.CalendarId) ||
                string.IsNullOrEmpty(settings.CollectionneurDabasePath))
            {
                this.logger.LogVerbose("No calendar id or database path defeined into the settings.");
                reason = CanNotSyncReason.BadSettings;
            }

            // test network connection
            if (!this.networkHelper.IsInternetConnectionAvailable())
            {
                this.logger.LogVerbose("No internet connection available");
                reason = CanNotSyncReason.NoNetwork;
            }

            // test if collector app is running
            if (this.IsCollectorReceiptAppRunning())
            {
                this.logger.LogVerbose("The collector app is running");
                reason = CanNotSyncReason.CollectorAppIsRunning;
            }

            this.logger.LogInformation(string.Format(
                "Can the application synchronize the menu? {0}", 
                Enum.GetName(typeof(CanNotSyncReason), reason)));

            return reason;
        }
    }
}
