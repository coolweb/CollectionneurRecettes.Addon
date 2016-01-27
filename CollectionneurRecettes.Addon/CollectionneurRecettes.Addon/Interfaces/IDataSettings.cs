// <copyright file="IDataSettings.cs" company="No company">
// 2016 GNU license
// </copyright>
// <author>Jordens Christophe</author>

namespace CollectionneurRecettes.Addon.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// The settings data layer.
    /// </summary>
    public interface IDataSettings
    {
        /// <summary>
        /// Reads the settings.
        /// </summary>
        /// <returns>The settings loaded.</returns>
        Entity.Settings ReadSettings();

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <exception cref="System.ArgumentNullException">settings is null</exception>
        void SaveSettings(Entity.Settings settings);
    }
}
