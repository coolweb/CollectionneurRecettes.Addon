// <copyright file="googlemanager.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Business
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CollectionneurRecettes.Addon.Entity;
    using CollectionneurRecettes.Addon.Interfaces;
    using Google.Apis.Calendar.v3.Data;

    /// <summary>
    /// Manage google account.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.IGoogleManager" />
    public class GoogleManager : Interfaces.IGoogleManager
    {
        /// <summary>
        /// The logger
        /// </summary>
        private CrossCutting.ILoggerService logger;

        /// <summary>
        /// The google repository
        /// </summary>
        private Interfaces.IGoogleRepository googleRepository;

        /// <summary>
        /// The settings manager
        /// </summary>
        private ISettingsManager settingsManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleManager"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="googleRepository">The google repository.</param>
        /// <param name="settingsManager">The settings manager.</param>
        /// <exception cref="System.ArgumentNullException">
        /// googleRepository
        /// or
        /// settingsManager
        /// or
        /// logger
        /// </exception>
        public GoogleManager(CrossCutting.ILoggerService logger, Interfaces.IGoogleRepository googleRepository, ISettingsManager settingsManager)
        {
            if (googleRepository == null)
            {
                throw new ArgumentNullException("googleRepository");
            }

            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.googleRepository = googleRepository;
            this.settingsManager = settingsManager;
            this.logger = logger;
        }

        /// <summary>
        /// Clears the configured account by deleting the data store file if exist.
        /// </summary>
        public void ClearConfiguredAccount()
        {
            if (this.IsAccountAlreadyConfigured())
            {
                this.logger.LogInformation("The google account is configured, delete data store file");
                this.googleRepository.DeleteDataStoreFile();
            }
        }

        /// <summary>
        /// Configures the google account.
        /// </summary>
        /// <returns>Return true.</returns>
        public async Task<bool> ConfigureAccount()
        {
            this.logger.LogInformation("Launching google authorization...");
            var result = await this.googleRepository.Authorize();

            return true;
        }

        /// <summary>
        /// Creates the menus into the google calendar, if event already exists, it delete it and create it again.
        /// </summary>
        /// <param name="menu">The menu to create.</param>
        /// <param name="progress">The progress of the creation.</param>
        /// <returns>The number of receipts created/updated</returns>
        public async Task<int> CreateMenus(Menu menu, IProgress<Entity.ProgressCreateMenus> progress)
        {
            var count = 0;
            var receiptToCreate = 0;
            var appName = "CollectionneurRecettes.Addon";
            var settings = this.settingsManager.LoadSettings();
            var credential = await this.googleRepository.Authorize();

            foreach (var day in menu.Days)
            {
                receiptToCreate += day.Receipts.Count;
            }

            this.logger.LogVerbose(string.Format("{0} receipts to create", receiptToCreate));

            if (progress != null)
            {
                progress.Report(new ProgressCreateMenus()
                {
                    ReceiptToCreate = receiptToCreate
                });
                this.logger.LogVerbose("Report progress of creation");
            }

            // Load existing items from google calendar.
            var extendedProperties = new Dictionary<string, string>();
            extendedProperties.Add("AppName", appName);
            var beginDate = DateTime.Now.AddDays(settings.DaysToSyncBefore * -1);
            var endDate = DateTime.Now.AddDays(settings.DaysToSyncAfter);

            this.logger.LogInformation(string.Format("Search existing events into google calendar between {0} and {1}", beginDate, endDate));
            var events = await this.googleRepository.FilterEvents(
                credential,
                settings.CalendarId,
                beginDate,
                endDate,
                extendedProperties);

            foreach (var day in menu.Days)
            {
                var daySummary = string.Empty;

                foreach (var receipt in day.Receipts)
                {
                    daySummary += receipt.Name + "\n";

                    count++;

                    if (progress != null)
                    {
                        this.logger.LogVerbose(string.Format("Report progress of creation, created {0}", count));
                        progress.Report(new ProgressCreateMenus()
                        {
                            ReceiptCreated = count,
                            ReceiptToCreate = receiptToCreate
                        });
                    }
                }

                if (daySummary != string.Empty)
                {
                    // search receipt in google calendar
                    var eventInGoogleCalendar = events.Items.Where((e) =>
                    {
                        return e.ExtendedProperties.Private__.Any(prop => prop.Key == "AppName" && prop.Value == appName) &&
                        e.Summary == "Menu du jour" &&
                        e.Start.Date == day.Date.ToString("yyyy-MM-dd") &&
                        e.End.Date == day.Date.ToString("yyyy-MM-dd");
                    });

                    bool mustCreateEvent = false;
                    if (!eventInGoogleCalendar.Any())
                    {
                        // event doesn't exist, must create it
                        mustCreateEvent = true;
                    }
                    else
                    {
                        // event exist, check if receipt have changed
                        if (eventInGoogleCalendar.First().Description != daySummary)
                        {
                            // menu have changed, so delete old event
                            this.logger.LogInformation(string.Format("Event {0} exist in calendar, delete it", daySummary));
                            this.googleRepository.DeleteEvent(credential, settings.CalendarId, eventInGoogleCalendar.First().Id);

                            // must create the updated event
                            mustCreateEvent = true;
                        }
                    }

                    if (mustCreateEvent)
                    {
                        // must create the event                       
                        var eventGoogle = new Event()
                        {
                            Start = new EventDateTime() { Date = day.Date.ToString("yyyy-MM-dd") },
                            End = new EventDateTime() { Date = day.Date.ToString("yyyy-MM-dd") },
                            Description = daySummary,
                            Summary = "Menu du jour",
                            ExtendedProperties = new Event.ExtendedPropertiesData()
                            {
                                Private__ = new Dictionary<string, string>()
                            }
                        };
                        eventGoogle.ExtendedProperties.Private__.Add("AppName", appName);

                        this.logger.LogInformation(string.Format("Create event {0} at {1} into google calendar", daySummary, day.Date));
                        var createdEvent = await this.googleRepository.CreateEvent(
                            credential,
                            settings.CalendarId,
                            eventGoogle);
                    }
                }
            }

            return count;
        }

        /// <summary>
        /// Determines whether [is account already configured].
        /// Check if data store file exists and secret file.
        /// </summary>
        /// <returns>[True] if account is configured, otherwise [False]</returns>
        public bool IsAccountAlreadyConfigured()
        {
            var isConfigured = this.googleRepository.FileDataStoreExists() &&
                this.googleRepository.SecretFileExists();
            this.logger.LogInformation(string.Format("Is google account configured: {0}", isConfigured));

            return isConfigured;
        }

        /// <summary>
        /// Determine if the secrets the file exists.
        /// </summary>
        /// <returns>[True] if secret file exists, otherwise [False]</returns>
        public bool SecretFileExists()
        {
            var secretFileExists = this.googleRepository.SecretFileExists();
            this.logger.LogInformation(string.Format("Google json secret file exists: {0}", secretFileExists));

            return secretFileExists;
        }

        /// <summary>
        /// Loads the calendars from the google account.
        /// </summary>
        /// <returns>A list of calendars</returns>
        public async Task<IEnumerable<Entity.Calendar>> LoadCalendars()
        {
            this.logger.LogInformation("Load google calendars");
            Google.Apis.Auth.OAuth2.UserCredential credential = await this.googleRepository.Authorize();
            return this.googleRepository.RetrieveCalendars(credential);
        }
    }
}
