using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Client.ViewModels
{
    public class ParametersViewModel : BindableBase
    {
        private short daysToSyncAfter;
        private short daysToSyncBefore;
        private bool canSave;
        private bool canCancel;
        private Entity.Settings currentSettings;
        private Entity.Calendar selectedCalendar;
        private string collectionneurRecetteDbPath = string.Empty;
        private Interfaces.ISettingsManager settingsManager;
        private Services.IInteractionService interactionService;
        private Services.IDispatcherService dispatcherService;
        private IEventAggregator eventAggregator;
        private Interfaces.IGoogleManager googleManager;
        private bool isGmailAccountAlreadyConfigured;
        private bool isLoading;

        public ParametersViewModel(
            Interfaces.ISettingsManager settingsManager,
            Services.IInteractionService interactionService,
            Services.IDispatcherService dispatcherService,
            Interfaces.IGoogleManager googleManager,
            IEventAggregator eventAggregator)
        {
            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            if (interactionService == null)
            {
                throw new ArgumentNullException("InteractionService");
            }

            if (dispatcherService == null)
            {
                throw new ArgumentNullException("dispatcherService");
            }

            if (eventAggregator == null)
            {
                throw new ArgumentNullException("EventAggregator");
            }

            if (googleManager == null)
            {
                throw new ArgumentNullException("googleManager");
            }

            this.settingsManager = settingsManager;
            this.interactionService = interactionService;
            this.dispatcherService = dispatcherService;
            this.eventAggregator = eventAggregator;
            this.googleManager = googleManager;

            this.Calendars = new ObservableCollection<Entity.Calendar>();

            this.SelectCollectionneurDbPathCommand = new DelegateCommand(new Action(() =>
            {
                this.SelectCollectionneurDbPath();
            }));

            this.SaveCommand = new DelegateCommand(
                new Action(this.Save),
                new Func<bool>(() => this.CanSave));

            this.IsGmailAccountAlreadyConfigured = this.googleManager.IsAccountAlreadyConfigured();

            this.currentSettings = this.settingsManager.LoadSettings();
            this.DaysToSyncBefore = this.currentSettings.DaysToSyncBefore;
            this.DaysToSyncAfter = this.currentSettings.DaysToSyncAfter;
            this.collectionneurRecetteDbPath = this.currentSettings.CollectionneurDabasePath;
        }

        public short DaysToSyncAfter
        {
            get
            {
                return this.daysToSyncAfter;
            }

            set
            {
                if(this.SetProperty(ref this.daysToSyncAfter, value))
                {
                    this.CanSave = true;
                    this.SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }
        
        public short DaysToSyncBefore
        {
            get
            {
                return this.daysToSyncBefore;
            }

            set
            {
                if(this.SetProperty(ref this.daysToSyncBefore, value))
                {
                    this.CanSave = true;
                    this.SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }


        public Entity.Calendar SelectedCalendar
        {
            get
            {
                return this.selectedCalendar;
            }

            set
            {
                if(base.SetProperty(ref this.selectedCalendar, value))
                {
                    this.CanSave = true;
                }
            }
        }


        public string CollectionneurRecetteDbPath
        {
            get
            {
                return this.collectionneurRecetteDbPath;
            }

            set
            {
                if(base.SetProperty(ref this.collectionneurRecetteDbPath, value))
                {
                    this.CheckCollectionneurDbPath(value);
                }
            }
        }
        
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                base.SetProperty(ref this.isLoading, value);
            }
        }

        public ObservableCollection<Entity.Calendar> Calendars
        {
            get;
            private set;
        }

        public bool IsGmailAccountAlreadyConfigured
        {
            get
            {
                return this.isGmailAccountAlreadyConfigured;
            }

            set
            {
                if(base.SetProperty(ref this.isGmailAccountAlreadyConfigured, value))
                {
                    if (value)
                    {
                        this.IsLoading = true;
                        this.googleManager.ConfigureAccount().ContinueWith(task=>
                        {
                            if (task.Status != TaskStatus.RanToCompletion)
                            {
                                this.interactionService.ShowErrorDialog("Erreur lors de la configuration du compte google!");
                                this.IsGmailAccountAlreadyConfigured = false;
                                this.IsLoading = false;
                            }
                            else
                            {
                                // Load calendars list
                                this.LoadCalendars();
                            }
                        });
                    }
                    else
                    {
                        if (!value && this.googleManager.IsAccountAlreadyConfigured())
                        {
                            this.googleManager.ClearConfiguredAccount();
                        }
                    }
                }
            }
        }

        public DelegateCommand SelectCollectionneurDbPathCommand
        {
            get;
            set;
        }

        public DelegateCommand SaveCommand
        {
            get;
            set;
        }

        public void LoadCalendars()
        {
            this.IsLoading = true;

            this.googleManager.LoadCalendars().ContinueWith(taskLoadCalendars =>
            {
                this.IsLoading = false;

                if (taskLoadCalendars.Status == TaskStatus.RanToCompletion)
                {
                    this.dispatcherService.Invoke(() =>
                    {
                        this.Calendars.Clear();
                        this.Calendars.AddRange(taskLoadCalendars.Result);

                        if (string.IsNullOrEmpty(this.currentSettings.CalendarId))
                        {
                            this.SelectedCalendar = this.Calendars.FirstOrDefault();
                        }
                        else
                        {
                            this.SelectedCalendar = this.Calendars.FirstOrDefault((c) => c.CalendarId == this.currentSettings.CalendarId);
                        }
                    });
                }
                else
                {
                    if (taskLoadCalendars.Status == TaskStatus.Faulted)
                    {
                        this.interactionService.ShowErrorDialog("Impossible de charger les calendriers google!");
                        this.IsGmailAccountAlreadyConfigured = false;
                    }
                }
            });
        }

        public void ClearConfiguredAccount()
        {
            this.googleManager.ClearConfiguredAccount();
            this.IsGmailAccountAlreadyConfigured = false;
            this.Calendars.Clear();
            this.SelectedCalendar = null;
        }

        public void LoadSettings()
        {
            var settings = this.settingsManager.LoadSettings();

            this.CollectionneurRecetteDbPath = settings.CollectionneurDabasePath;
        }
        
        public bool CanSave
        {
            get
            {
                return this.canSave;
            }

            set
            {
                if(!base.SetProperty(ref this.canSave, value))
                {
                    this.SaveCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool CanCancel
        {
            get
            {
                return this.canCancel;
            }

            set
            {
                base.SetProperty(ref this.canCancel, value);
            }
        }
       
        private void Save()
        {
            if (this.SelectedCalendar != null)
            {
                this.currentSettings.CalendarId = this.SelectedCalendar.CalendarId;
            }

            this.currentSettings.CollectionneurDabasePath = this.CollectionneurRecetteDbPath;
            this.currentSettings.DaysToSyncAfter = this.DaysToSyncAfter;
            this.currentSettings.DaysToSyncBefore = this.DaysToSyncBefore;

            this.settingsManager.SaveSettings(this.currentSettings);
        }

        private void Cancel()
        {
            if (!string.IsNullOrEmpty(this.currentSettings.CalendarId))
            {
                this.SelectedCalendar = this.Calendars.FirstOrDefault(c => c.CalendarId == this.currentSettings.CalendarId);
            }

            this.CollectionneurRecetteDbPath = this.currentSettings.CollectionneurDabasePath;
        }

        private void SelectCollectionneurDbPath()
        {
            var path = this.interactionService.OpenFileDialog("collector db|collector.h2.db");

            this.CollectionneurRecetteDbPath = path;
        }

        private void CheckCollectionneurDbPath(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                // check db path is valid
                if (this.settingsManager.IsCollectionneurRecetteDbPathValid(path))
                {
                    this.CanSave = true;
                    this.interactionService.ShowSuccessDialog("Test de connexion à la base de donnée réussi!");
                    this.SaveCommand.RaiseCanExecuteChanged();
                }
                else
                {
                    this.CollectionneurRecetteDbPath = string.Empty;
                    this.interactionService.ShowErrorDialog("Le chemin spécifié n'est pas valide ou bien l'application Collectionneur de recettes est cours.");
                }
            }
        }
    }
}
