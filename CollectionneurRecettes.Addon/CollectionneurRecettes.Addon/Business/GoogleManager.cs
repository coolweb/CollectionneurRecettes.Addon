﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionneurRecettes.Addon.Entity;
using Google.Apis.Calendar.v3.Data;
using CollectionneurRecettes.Addon.Interfaces;

namespace CollectionneurRecettes.Addon.Business
{
    public class GoogleManager : Interfaces.IGoogleManager
    {
        private Interfaces.IGoogleRepository googleRepository;
        private ISettingsManager settingsManager;

        public GoogleManager(Interfaces.IGoogleRepository googleRepository, ISettingsManager settingsManager)
        {
            if (googleRepository == null)
            {
                throw new ArgumentNullException("googleRepository");
            }

            if (settingsManager == null)
            {
                throw new ArgumentNullException("settingsManager");
            }

            this.googleRepository = googleRepository;
            this.settingsManager = settingsManager;
        }

        public void ClearConfiguredAccount()
        {
            if (this.IsAccountAlreadyConfigured())
            {
                this.googleRepository.DeleteDataStoreFile();
            }
        }

        public async Task<bool> ConfigureAccount()
        {
            var result = await this.googleRepository.Authorize();

            return true;
        }

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

            if (progress != null)
            {
                progress.Report(new ProgressCreateMenus()
                {
                    ReceiptToCreate = receiptToCreate
                });
            }

            // Load existing items from google calendar.
            var extendedProperties = new Dictionary<string, string>();
            extendedProperties.Add("AppName", appName);
            var events = await this.googleRepository.FilterEvents(
                credential,
                settings.CalendarId,
                DateTime.Now.AddDays(settings.DaysToSyncBefore * -1),
                DateTime.Now.AddDays(settings.DaysToSyncAfter),
                extendedProperties);


            foreach (var day in menu.Days)
            {
                var daySummary = string.Empty;

                foreach (var receipt in day.Receipts)
                {
                    daySummary += receipt.Name +"\n";
                    
                    count++;

                    if (progress != null)
                    {
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
                            googleRepository.DeleteEvent(credential, settings.CalendarId, eventInGoogleCalendar.First().Id);

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

                        var createdEvent = await this.googleRepository.CreateEvent(
                            credential,
                            settings.CalendarId,
                            eventGoogle);
                    }
                }
            }

            return count;
        }

        public bool IsAccountAlreadyConfigured()
        {
            return this.googleRepository.FileDataStoreExists() &&
                this.googleRepository.SecretFileExists();
        }

        public bool SecretFileExists()
        {
            return this.googleRepository.SecretFileExists();
        }

        public async Task<IEnumerable<Entity.Calendar>> LoadCalendars()
        {
            Google.Apis.Auth.OAuth2.UserCredential credential = await this.googleRepository.Authorize();
            return this.googleRepository.RetrieveCalendars(credential);
        }
    }
}
