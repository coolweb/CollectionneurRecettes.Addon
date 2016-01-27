// <copyright file="GoogleRepository.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Repository
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Calendar.v3;
    using Google.Apis.Calendar.v3.Data;
    using Google.Apis.Util.Store;

    /// <summary>
    /// The google data layer.
    /// </summary>
    /// <seealso cref="CollectionneurRecettes.Addon.Interfaces.IGoogleRepository" />
    public class GoogleRepository : Interfaces.IGoogleRepository
    {
        /// <summary>
        /// The logger
        /// </summary>
        private CrossCutting.ILoggerService logger;

        /// <summary>
        /// The google application name
        /// </summary>
        private string googleApplicationName = "collectionneurrecettesaddon";

        /// <summary>
        /// The file data store full path
        /// </summary>
        private string fileDataStoreFullPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CollectionneurRecettesAddon.GoogleStore\\Google.Apis.Auth.OAuth2.Responses.TokenResponse-user");

        /// <summary>
        /// The client secret file
        /// </summary>
        private string clientSecretFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CollectionneurRecettes.Addon\\client_secrets.json");

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleRepository"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <exception cref="System.ArgumentNullException">logger is null</exception>
        public GoogleRepository(CrossCutting.ILoggerService logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this.logger = logger;
        }

        /// <summary>
        /// Authorizes this instance to google
        /// </summary>
        /// <returns>The user credential.</returns>
        public async Task<UserCredential> Authorize()
        {
            this.logger.LogInformation("Authorize user to google API");
            UserCredential credential;
            using (var stream = new FileStream(this.clientSecretFile, FileMode.Open, FileAccess.Read))
            {
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { CalendarService.Scope.Calendar },
                    "user", 
                    CancellationToken.None,
                    new FileDataStore("CollectionneurRecettesAddon.GoogleStore"));

                return credential;
            }
        }

        /// <summary>
        /// Deletes the data store file.
        /// </summary>
        public void DeleteDataStoreFile()
        {
            this.logger.LogInformation(string.Format("Delete data store file {0}", this.fileDataStoreFullPath));
            File.Delete(this.fileDataStoreFullPath);
        }

        /// <summary>
        /// Determine if the data store file exists.
        /// </summary>
        /// <returns>true if data store file exists, otherwise false.</returns>
        public bool FileDataStoreExists()
        {
            var exists = File.Exists(this.fileDataStoreFullPath);
            this.logger.LogInformation(string.Format("File data store {0} exists: {1}", this.fileDataStoreFullPath, exists));

            return exists;
        }

        /// <summary>
        /// Determine if secrets the file exists.
        /// </summary>
        /// <returns>true if secret file exists, otherwise false.</returns>
        public bool SecretFileExists()
        {
            var exists = File.Exists(this.clientSecretFile);
            this.logger.LogInformation(string.Format("File {0} exists: {1}", this.clientSecretFile, exists));

            return exists;
        }

        /// <summary>
        /// Retrieves the calendars.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <returns>A list of calendars</returns>
        /// <exception cref="System.ArgumentNullException">credential is null</exception>
        public IEnumerable<Entity.Calendar> RetrieveCalendars(UserCredential credential)
        {
            if (credential == null)
            {
                throw new ArgumentNullException("credential");
            }

            this.logger.LogInformation("Load google calendars");
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
                    this.logger.LogVerbose("Load next calendar page");
                    calendarListResource.PageToken = calendarList.NextPageToken;
                    calendarList = calendarListResource.Execute();
                }
                else
                {
                    // no next page
                    this.logger.LogVerbose("No more calendar page to load");
                    nextCalendar = false;
                }
            }

            this.logger.LogInformation(string.Format("{0} calendars loaded", calendars.Count));
            return calendars;
        }

        /// <summary>
        /// Creates the event.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <param name="calendarId">The calendar identifier.</param>
        /// <param name="eventToCreate">The event to create.</param>
        /// <returns>The created event</returns>
        /// <exception cref="System.ArgumentNullException">
        /// credential
        /// or
        /// calendarId
        /// or
        /// eventToCreate
        /// </exception>
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
            this.logger.LogInformation(string.Format("Create event {0} into calendar {1}", eventToCreate.Description, calendarId));

            return request.ExecuteAsync();
        }

        /// <summary>
        /// Filters the events between two dates.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <param name="calendarId">The calendar identifier.</param>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="extendedProperties">The extended properties.</param>
        /// <returns>The found events</returns>
        /// <exception cref="System.ArgumentNullException">
        /// credential
        /// or
        /// calendarId
        /// </exception>
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

            this.logger.LogInformation(string.Format("Filter calendar events from calendar {0} between {1} and {2}", calendarId, filter.TimeMin, filter.TimeMax));
            return await filter.ExecuteAsync();
        }

        /// <summary>
        /// Deletes the event.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <param name="calendarId">The calendar identifier.</param>
        /// <param name="eventId">The event identifier.</param>
        /// <exception cref="System.ArgumentNullException">
        /// credential
        /// or
        /// calendarId
        /// or
        /// eventId
        /// </exception>
        public void DeleteEvent(UserCredential credential, string calendarId, string eventId)
        {
            if (credential == null)
            {
                throw new ArgumentNullException("credential");
            }

            if (string.IsNullOrEmpty(calendarId))
            {
                throw new ArgumentNullException("calendarId");
            }

            if (string.IsNullOrEmpty(eventId))
            {
                throw new ArgumentNullException("eventId");
            }

            var service = new CalendarService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = this.googleApplicationName
            });

            this.logger.LogInformation(string.Format("Delete event id {0} from calendar {1}", eventId, calendarId));
            service.Events.Delete(calendarId, eventId).Execute();
        }
    }
}
