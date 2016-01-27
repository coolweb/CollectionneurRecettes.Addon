// <copyright file="ISettingsManager.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Interfaces
{
    /// <summary>
    /// The manager for the settings of the application.
    /// </summary>
    public interface ISettingsManager
    {
        /// <summary>
        /// Loads the settings.
        /// </summary>
        /// <returns>A setting of object.</returns>
        Entity.Settings LoadSettings();

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">settings is null</exception>
        void SaveSettings(Entity.Settings settings);

        /// <summary>
        /// Determines whether database path is valid.
        /// </summary>
        /// <param name="path">The path of the database.</param>
        /// <returns>True if path is valid, otherwise false.</returns>
        bool IsCollectionneurRecetteDbPathValid(string path);
    }
}
