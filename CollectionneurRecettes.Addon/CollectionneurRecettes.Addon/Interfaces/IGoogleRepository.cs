using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionneurRecettes.Addon.Interfaces
{
    public interface IGoogleRepository
    {
        bool FileDataStoreExists();
        bool SecretFileExists();
        void DeleteDataStoreFile();
        Task<UserCredential> Authorize();
        IEnumerable<Entity.Calendar> RetrieveCalendars(UserCredential credential);
        Task<Event> CreateEvent(UserCredential credential, string calendarId, Event eventToCreate);
        Task<Events> FilterEvents(UserCredential credential, string calendarId, DateTime startDate, DateTime endDate, IDictionary<string, string> extendedProperties);
        void DeleteEvent(UserCredential credential, string calendarId, string eventId);
    }
}
