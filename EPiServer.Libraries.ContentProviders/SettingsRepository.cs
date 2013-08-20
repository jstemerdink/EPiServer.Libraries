// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsRepository.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using EPiServer.Data.Dynamic;
    using EPiServer.Libraries.ContentProviders.Models;

    /// <summary>
    /// The settings repository.
    /// </summary>
    public class SettingsRepository
    {
        #region Static Fields

        /// <summary>
        ///     The synclock object.
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        ///     The one and only SettingsRepository instance.
        /// </summary>
        private static volatile SettingsRepository instance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="SettingsRepository" /> class from being created.
        /// </summary>
        private SettingsRepository()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance of the SettingsRepository object.
        /// </summary>
        public static SettingsRepository Instance
        {
            get
            {
                // Double checked locking
                if (instance == null)
                {
                    lock (SyncLock)
                    {
                        if (instance == null)
                        {
                            instance = new SettingsRepository();
                        }
                    }
                }

                return instance;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the store.
        /// </summary>
        private static DynamicDataStore Store
        {
            get
            {
                return typeof(ClonedContentProviderSettings).GetStore();
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The load settings.
        /// </summary>
        /// <returns>
        /// The <see cref="ClonedContentProviderSettings"/>.
        /// </returns>
        public Collection<ClonedContentProviderSettings> GetAll()
        {
            return new Collection<ClonedContentProviderSettings>(Store.Items<ClonedContentProviderSettings>().ToList());
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// The <see cref="ClonedContentProviderSettings"/>.
        /// </returns>
        public ClonedContentProviderSettings Get(string name)
        {
            return Store.Items<ClonedContentProviderSettings>().SingleOrDefault(cp => cp.Name == name);
        }

        /// <summary>
        /// Deletes the provider setting.
        /// </summary>
        /// <param name="providerSettings">The provider settings.</param>
        /// <returns>Whether the delete action succeeded.</returns>
        public bool Delete(ClonedContentProviderSettings providerSettings)
        {
            try
            {
                Store.Delete(providerSettings);
            }
            catch (InvalidOperationException)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Deletes all provider settings.
        /// </summary>
        public void DeleteAll()
        {
            Store.DeleteAll();
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <returns>Whether the save action succeeded.</returns>
        public bool SaveSettings(ClonedContentProviderSettings settings)
        {
            if (settings == null)
            {
                return false;
            }

            try
            {
                ClonedContentProviderSettings currentSettings = this.Get(settings.Name);

                if (currentSettings == null)
                {
                    Store.Save(settings);
                }
                else
                {
                    Store.Save(settings, currentSettings.Id); 
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}