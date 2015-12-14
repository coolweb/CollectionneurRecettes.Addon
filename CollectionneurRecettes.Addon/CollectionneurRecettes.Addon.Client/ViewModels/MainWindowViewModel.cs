using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private Interfaces.ISettingsManager settingsManager;
        private Services.IDispatcherService dispatcherService;
        private DelegateCommand exitCommand;
        private DelegateCommand showParametersCommand;
        private Interfaces.ICollectorReceiptManager collectorReceiptManager;
        private Interfaces.IGoogleManager googleManager;
        private IEventAggregator eventAggregator;
        private Timer checkAppRunningTimer;
        private bool isAppCollectorAlreadyRunning;

        public MainWindowViewModel(
            Interfaces.ICollectorReceiptManager collectorReceiptManager,
            Interfaces.ISettingsManager settingsManager,
            Interfaces.IGoogleManager googleManager,
            Services.IDispatcherService dispatcherService,
            IEventAggregator eventAggregator)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            if (dispatcherService == null)
            {
                throw new ArgumentNullException("dispatcherService");
            }

            if (collectorReceiptManager == null)
            {
                throw new ArgumentNullException("collectorReceiptManager");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            if (googleManager == null)
            {
                throw new ArgumentNullException("googleManager");
            }

            this.exitCommand = new DelegateCommand(new Action(() =>
            {
                System.Windows.Application.Current.Shutdown();
            }));

            this.showParametersCommand = new DelegateCommand(new Action(() =>
            {
                var regionManager = ServiceLocator.Current.GetInstance<Prism.Regions.IRegionManager>();
                regionManager.RequestNavigate("AppBarRegion", new Uri("AppBarParametersView", UriKind.Relative));
                regionManager.RequestNavigate("MainRegion", new Uri("ParametersView", UriKind.Relative));
                this.eventAggregator.GetEvent<Events.ActivateWindowEvent>().Publish(null);
            }));

            this.SyncMenuCommand = new DelegateCommand(() =>
            {
                var regionManager = ServiceLocator.Current.GetInstance<Prism.Regions.IRegionManager>();
                //regionManager.RequestNavigate("AppBarRegion", new Uri("AppBarParametersView", UriKind.Relative));
                regionManager.RequestNavigate("MainRegion", new Uri("SyncMenuView", UriKind.Relative));
                this.eventAggregator.GetEvent<Events.ActivateWindowEvent>().Publish(null);
                this.eventAggregator.GetEvent<Events.StartSyncEvent>().Publish(null);
            });

            this.ShowAboutCommand = new DelegateCommand(() =>
            {
                var regionManager = ServiceLocator.Current.GetInstance<Prism.Regions.IRegionManager>();
                regionManager.RequestNavigate("MainRegion", new Uri("AboutView", UriKind.Relative));
                this.eventAggregator.GetEvent<Events.ActivateWindowEvent>().Publish(null);
            });

            this.eventAggregator = eventAggregator;
            this.collectorReceiptManager = collectorReceiptManager;
            this.googleManager = googleManager;
            this.settingsManager = settingsManager;
            this.dispatcherService = dispatcherService;

            this.LoadCommand = new DelegateCommand(() =>
            {
                if (string.IsNullOrEmpty(this.settingsManager.LoadSettings().CollectionneurDabasePath))
                {
                    this.dispatcherService.Invoke(() =>
                    {
                        this.ShowParametersCommand.Execute();
                    });
                }

                this.checkAppRunningTimer = new Timer((state) =>
                {
                    if (this.collectorReceiptManager.CanSync() == Entity.CanNotSyncReason.None)
                    {
                        if (this.collectorReceiptManager.IsCollectorReceiptAppRunning())
                        {
                            if (!this.isAppCollectorAlreadyRunning)
                            {
                                // the app is not running before, so show notification
                                this.eventAggregator.GetEvent<Events.DisplayNotificationMessageEvent>().Publish("Application collectionneur de recettes détectées, en attente de fermeture pour synchronisation");
                                this.isAppCollectorAlreadyRunning = true;
                            }
                        }
                        else
                        {
                            if (this.isAppCollectorAlreadyRunning)
                            {
                                // the app was running, so launch sync
                                // TODO: launch sync
                                var regionManager = ServiceLocator.Current.GetInstance<Prism.Regions.IRegionManager>();
                                regionManager.RequestNavigate("MainRegion", new Uri("SyncMenuView", UriKind.Relative));
                                this.eventAggregator.GetEvent<Events.StartSyncEvent>().Publish(null);
                                this.eventAggregator.GetEvent<Events.DisplayNotificationMessageEvent>().Publish("Application collectionneur de recettes fermée, début de la synchronisation");
                                this.isAppCollectorAlreadyRunning = false;
                            }
                        }
                    }

                    this.checkAppRunningTimer.Change(2000, Timeout.Infinite);
                },
                null,
                1000,
                Timeout.Infinite);

                this.dispatcherService.Invoke(() =>
                {
                    if (!this.googleManager.SecretFileExists())
                    {
                        this.eventAggregator.GetEvent<Events.DisplayErrorMessageEvent>().Publish(new Events.DisplayErrorMessageEventArgs()
                        {
                            Message = "Veuillez créer votre fichier client_secrets.json, placez le dans " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\CollectionneurRecettes.Addon\n et redémarrez l'application",
                            CloseApplication = true
                        });
                    }
                });
            });
        }

        public DelegateCommand LoadCommand { get; set; }

        public DelegateCommand ExitCommand
        {
            get { return exitCommand; }
            set { exitCommand = value; }
        }

        public DelegateCommand ShowParametersCommand
        {
            get { return showParametersCommand; }
            set { showParametersCommand = value; }
        }

        public DelegateCommand SyncMenuCommand { get; set; }
        public DelegateCommand ShowAboutCommand { get; set; }

        private void Dispose(bool isDisposing)
        {
            // TODO: review dispose pattern
            if (isDisposing)
            {
                if (this.checkAppRunningTimer != null)
                {
                    this.checkAppRunningTimer.Dispose();
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
    }
}
