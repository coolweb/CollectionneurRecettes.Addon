using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CollectionneurRecettes.Addon.Client.ViewModels
{
    public class SyncMenuViewModel : BindableBase
    {
        private string syncState;
        private int syncSteps;
        private int currentSyncStep;
        private bool isSyncing;

        private Interfaces.ICollectorReceiptManager receiptManager;
        private Interfaces.IGoogleManager googleManager;
        private IEventAggregator eventAggregator;

        public SyncMenuViewModel(
            Interfaces.ICollectorReceiptManager receiptManager, 
            Interfaces.IGoogleManager googleManager,
            IEventAggregator eventAggregator)
        {
            if (receiptManager == null)
            {
                throw new ArgumentNullException("receiptManager");
            }

            if (googleManager == null)
            {
                throw new ArgumentNullException("googleManager");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }

            this.eventAggregator = eventAggregator;
            this.receiptManager = receiptManager;
            this.googleManager = googleManager;

            this.eventAggregator.GetEvent<Events.StartSyncEvent>().Subscribe((o) =>
            {
                this.StartSync();
            });       
        }

        public bool IsSyncing
        {
            get
            {
                return this.isSyncing;
            }

            set
            {
                this.SetProperty(ref this.isSyncing, value);
            }
        }


        public int SyncSteps
        {
            get
            {
                return this.syncSteps;
            }

            set
            {
                this.SetProperty(ref this.syncSteps, value);
            }
        }

        public int CurrentSyncStep
        {
            get
            {
                return this.currentSyncStep;
            }

            set
            {
                this.SetProperty(ref this.currentSyncStep, value);
            }
        }


        public string SyncState
        {
            get
            {
                return this.syncState;
            }

            set
            {
                this.SetProperty(ref this.syncState, value);
            }
        }


        public void StartSync()
        {
            if (!this.IsSyncing)
            {
                var canSync = this.receiptManager.CanSync();
                if (canSync == Entity.CanNotSyncReason.None)
                {
                    this.IsSyncing = true;
                    this.SyncState = "Début de la synchronisation...";
                    this.CurrentSyncStep = 0;
                    this.SyncSteps = 0;

                    var progress = new Progress<Entity.ProgressCreateMenus>((prg) =>
                    {
                        this.SyncSteps = prg.ReceiptToCreate;
                        this.CurrentSyncStep = prg.ReceiptCreated;
                    });

                    this.SyncState = "Recherche du menu dans le collectionneur de recettes...";
                    var menu = this.receiptManager.RetrieveMenu();

                    if (menu.Days.Any())
                    {
                        this.SyncState = "Création du menu dans votre calendrier Google...";
                        this.googleManager.CreateMenus(menu, progress).ContinueWith((result) =>
                        {
                            if (result.Exception != null)
                            {
                                this.SyncState = "Erreur de la synchronisation";
                                this.eventAggregator.GetEvent<Events.DisplayErrorMessageEvent>().Publish("Impossible de créer le menu dans votre calendrier google, vérifier que vous avez une connexion réseau et que vos paramètres soient correctes.");
                            }
                            else
                            {
                                this.SyncState = "Synchronisation terminée avec succès";
                            }

                            this.IsSyncing = false;
                        });
                    }
                    else
                    {
                        this.SyncState = "Aucun menu a synchroniser";
                        this.IsSyncing = false;
                    }
                }
                else
                {
                    var errorMsg = string.Empty;

                    switch (canSync)
                    {
                        case Entity.CanNotSyncReason.NoNetwork:
                            errorMsg = "Une connection internet est nécessaire";
                            break;
                        case Entity.CanNotSyncReason.CollectorAppIsRunning:
                            errorMsg = "L'application collectionneur de recettes doit être fermée";
                            break;
                        case Entity.CanNotSyncReason.BadSettings:
                            errorMsg = "L'application doit être paramêtrée correctement";
                            break;
                        default:
                            errorMsg = "Raison inconnue";
                            break;
                    }

                    this.eventAggregator.GetEvent<Events.DisplayErrorMessageEvent>().Publish(errorMsg);
                }
            }
        }
    }
}
