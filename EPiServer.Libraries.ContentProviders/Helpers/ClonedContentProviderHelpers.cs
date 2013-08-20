// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClonedContentProviderHelpers.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders.Helpers
{
    using System;
    using System.Globalization;
    using System.Linq;

    using EPiServer.Core;
    using EPiServer.Libraries.ContentProviders.Models;
    using EPiServer.Libraries.ContentProviders.Providers;
    using EPiServer.ServiceLocation;

    using log4net;

    /// <summary>
    ///     Helpers for the content providers
    /// </summary>
    public static class ClonedContentProviderHelpers
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="ClonedContentProviderHelpers" />
        ///     class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ClonedContentProviderHelpers));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Adds the provider.
        /// </summary>
        /// <param name="root">The root.</param>
        /// <param name="entryPoint">The entry point.</param>
        /// <param name="providerName">Name of the provider.</param>
        /// <param name="categoryList">The category list.</param>
        /// <returns>
        ///   <c>true</c> if [the provider has been attached]; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">Content provider for this node already attached.
        /// or
        /// Content provider with same name already attached.</exception>
        public static bool AddProvider(int root, int entryPoint, string providerName, CategoryList categoryList)
        {
            string name = string.Format(CultureInfo.InvariantCulture, "{0}-ClonedContent-{1}-{2}", providerName, root, entryPoint);

            IQueryable<ClonedContentProviderSettings> providerCollection = SettingsRepository.Instance.GetAll().AsQueryable();

            if (providerCollection.Count(pc => pc.EntryPoint.Equals(entryPoint)) > 0)
            {
                // A provider is already attached to this node.
                throw new InvalidOperationException("Content provider for this node already attached.");
            }

            if (providerCollection.Count(pc => pc.Name.Equals(name)) > 0)
            {
                // A provider with the same name already exists.
                throw new InvalidOperationException("Content provider with same name already attached.");
            }

            CategoryList categories = categoryList ?? new CategoryList();

            ClonedContentProvider provider = new ClonedContentProvider(
                name, 
                new PageReference(root), 
                new PageReference(entryPoint), 
                categories);

            IContentProviderManager providerManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();

            ClonedContentProviderSettings contentProviderSettings = new ClonedContentProviderSettings
                                                                  {
                                                                      Name = name, 
                                                                      EntryPoint = entryPoint, 
                                                                      Root = root, 
                                                                      CategoryList =
                                                                          string.Join(
                                                                              ",", 
                                                                              categories)
                                                                  };

            providerManager.ProviderMap.AddProvider(provider);

            SettingsRepository.Instance.SaveSettings(contentProviderSettings);

            CacheManager.Clear();

            return true;
        }

        /// <summary>
        /// Deletes the provider.
        /// </summary>
        /// <param name="providerName">Name of the provider.</param>
        /// <returns> <c>true</c> if [the provider was removed]; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Provider setting not found.
        /// or
        /// </exception>
        public static bool DeleteProvider(string providerName)
        {
            IContentProviderManager providerManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();
            ContentProvider pageProvider = providerManager.ProviderMap.GetProvider(providerName);

            try
            {
                if (pageProvider != null)
                {
                    providerManager.ProviderMap.RemoveProvider(providerName);
                }

                ClonedContentProviderSettings providerSetting = SettingsRepository.Instance.Get(providerName);

                if (providerSetting != null)
                {
                    SettingsRepository.Instance.Delete(providerSetting);
                }

                CacheManager.Clear();
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message, exception);
                throw new InvalidOperationException(exception.Message);
            }

            return true;
        }

        #endregion
    }
}