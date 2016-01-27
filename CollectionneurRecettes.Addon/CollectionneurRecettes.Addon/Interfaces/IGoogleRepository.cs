// <copyright file="IGoogleRepository.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Calendar.v3.Data;

    /// <summary>
    /// The google data layer.
    /// </summary>
    public interface IGoogleRepository
    {
        /// <summary>
        /// Determine if the data store file exists.
        /// </summary>
        /// <returns>true if data store file exists, otherwise false.</returns>
        bool FileDataStoreExists();

        /// <summary>
        /// Determine if secrets the file exists.
        /// </summary>
        /// <returns>true if secret file exists, otherwise false.</returns>
        bool SecretFileExists();

        /// <summary>
        /// Deletes the data store file.
        /// </summary>
        void DeleteDataStoreFile();

        /// <summary>
        /// Authorizes this instance to google
        /// </summary>
        /// <returns>The user credential.</returns>
        Task<UserCredential> Authorize();

        /// <summary>
        /// Retrieves the calendars.
        /// </summary>
        /// <param name="credential">The credential.</param>
        /// <returns>A list of calendars</returns>
        /// <exception cref="System.ArgumentNullException">credential is null</exception>
        IEnumerable<Entity.Calendar> RetrieveCalendars(UserCredential credential);

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
        Task<Event> CreateEvent(UserCredential credential, string calendarId, Event eventToCreate);

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
        Task<Events> FilterEvents(UserCredential credential, string calendarId, DateTime startDate, DateTime endDate, IDictionary<string, string> extendedProperties);

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
        void DeleteEvent(UserCredential credential, string calendarId, string eventId);
    }
}
