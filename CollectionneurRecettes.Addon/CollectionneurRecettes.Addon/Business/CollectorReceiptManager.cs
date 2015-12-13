using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionneurRecettes.Addon.Entity;
using AutoMapper;
using System.Diagnostics;
using CollectionneurRecettes.Addon.Interfaces;

namespace CollectionneurRecettes.Addon.Business
{
    public class CollectorReceiptManager : Interfaces.ICollectorReceiptManager
    {
        private string dbConnectionString;
        private Entity.Settings settings;
        private Interfaces.ISettingsManager settingsManager;
        private Interfaces.ICollectorReceiptRepository receiptRepository;
        private Interfaces.IProcessRepository processRepository;
        private Process collectorReceiptProcess;
        Interfaces.INetworkHelper networkHelper;

        public CollectorReceiptManager(
            Interfaces.ICollectorReceiptRepository receiptRepository, 
            Interfaces.IProcessRepository processRepository,
            Interfaces.ISettingsManager settingsManager,
            Interfaces.INetworkHelper networkHelper)
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

            this.receiptRepository = receiptRepository;
            this.processRepository = processRepository;
            this.settingsManager = settingsManager;
            this.settings = this.settingsManager.LoadSettings();
            this.networkHelper = networkHelper;

            this.dbConnectionString = "jdbc:h2:" + this.settings.CollectionneurDabasePath.Replace(".h2.db", string.Empty);
        }

        public Menu RetrieveMenu()
        {
            var data = this.receiptRepository.SelectMenu(
                this.dbConnectionString,
                DateTime.Now.AddDays(this.settings.DaysToSyncBefore * -1),
                DateTime.Now.AddDays(this.settings.DaysToSyncAfter));

            return Mapper.Map<Menu>(data);
        }

        public bool IsCollectorReceiptAppRunning()
        {
            if (this.collectorReceiptProcess == null)
            {
                this.collectorReceiptProcess = this.processRepository.RunningProcess("collectionneurderecettes");

                if (this.collectorReceiptProcess != null)
                {
                    this.collectorReceiptProcess.EnableRaisingEvents = true;
                    this.collectorReceiptProcess.Exited += (sender, args) => 
                    {
                        this.collectorReceiptProcess = null;
                    };
                }
            }

            return this.collectorReceiptProcess != null && !this.collectorReceiptProcess.HasExited;
        }

        public CanNotSyncReason CanSync()
        {
            // test settings
            var settings = this.settingsManager.LoadSettings();

            if (string.IsNullOrEmpty(settings.CalendarId) ||
                string.IsNullOrEmpty(settings.CollectionneurDabasePath))
            {
                return CanNotSyncReason.BadSettings;
            }

            // test network connection
            if (!this.networkHelper.IsInternetConnectionAvailable())
            {
                return CanNotSyncReason.NoNetwork;
            }

            // test if collector app is running
            if (this.IsCollectorReceiptAppRunning())
            {
                return CanNotSyncReason.CollectorAppIsRunning;
            }

            return CanNotSyncReason.None;
        }
    }
}
