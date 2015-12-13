using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using System.IO;
using System.Threading;
using Google.Apis.Util.Store;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;

namespace CollectionneurRecettes.Addon.Repository
{
    public class GoogleRepository : Interfaces.IGoogleRepository
    {
        private string googleApplicationName = "collectionneurrecettesaddon";
        private string fileDataStoreFullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CollectionneurRecettesAddon.GoogleStore\\Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");
        private string clientSecretFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CollectionneurRecettes.Addon\\client_secrets.json");

        public async Task<UserCredential> Authorize()
        {
            UserCredential credential;
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { CalendarService.Scope.Calendar },
                    "user", CancellationToken.None, new FileDataStore("CollectionneurRecettesAddon.GoogleStore"));

                return credential;
            }
        }

        public void DeleteDataStoreFile()
        {
            File.Delete(this.fileDataStoreFullPath);
        }

        public bool FileDataStoreExists()
        {
            return File.Exists(this.fileDataStoreFullPath);
        }

        public bool SecretFileExists()
        {
            return File.Exists(this.clientSecretFile);
        }

        public IEnumerable<Entity.Calendar> RetrieveCalendars(UserCredential credential)
        {
            if (credential == null)
            {
                throw new ArgumentNullException("credential");
            }

            var service = new CalendarService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.googleApplicationName
            });

            var nextCalendar = true;
            var calendarListResource = service.CalendarList.List();
            var calendarList = calendarListResource.Execute();
            var calendars = new List<Entity.Calendar>();
            
            while (nextCalendar)
            {
                calendars.AddRange(
                    calendarList.Items.Select((calendar) =>
                    {
                        return new Entity.Calendar()
                        {
                            CalendarId = calendar.Id,
                            Name = calendar.Summary
                        };
                    }));
                
                if (!string.IsNullOrEmpty(calendarList.NextPageToken))
                {
                    // next page
                    calendarListResource.PageToken = calendarList.NextPageToken;
                    calendarList = calendarListResource.Execute();
                }
                else
                {
                    // no next page
                    nextCalendar = false;
                }
            }

            return calendars;
        }

        public Task<Event> CreateEvent(UserCredential credential, string calendarId, Event eventToCreate)
        {
            if (credential == null)
            {
                throw new ArgumentNullException("credential");
            }

            if (calendarId == null)
            {
                throw new ArgumentNullException("calendarId");
            }

            if (eventToCreate == null)
            {
                throw new ArgumentNullException("eventToCreate");
            }

            var service = new CalendarService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.googleApplicationName
            });
            
            var request = service.Events.Insert(eventToCreate, calendarId);
            return request.ExecuteAsync();
        }

        public async Task<Events> FilterEvents(UserCredential credential, string calendarId, DateTime startDate, DateTime endDate, IDictionary<string, string> extendedProperties)
        {
            if (credential == null)
            {
                throw new ArgumentNullException("credential");
            }

            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentNullException("calendarId");
            }

            var service = new CalendarService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.googleApplicationName
            });

            var filter = service.Events.List(calendarId);

            if (extendedProperties != null)
            {
                filter.PrivateExtendedProperty = new Google.Apis.Util.Repeatable<string>(extendedProperties.Select(property => property.Key + "=" + property.Value));
            }

            filter.TimeMax = endDate.AddDays(1);
            filter.TimeMin = startDate.AddDays(-1);

            return await filter.ExecuteAsync();
        }
    }
}
