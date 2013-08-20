// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClonedContentProviderInitialization.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using EPiServer.Core;
    using EPiServer.Events;
    using EPiServer.Events.Clients;
    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Libraries.ContentProviders.Models;
    using EPiServer.Libraries.ContentProviders.Providers;
    using EPiServer.ServiceLocation;

    using log4net;

    /// <summary>
    ///     The initialization module for the translation provider.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class ClonedContentProviderInitialization : IInitializableModule
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="ClonedContentProviderInitialization" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ClonedContentProviderInitialization));

        /// <summary>
        ///     Check if the initialization has been done.
        /// </summary>
        private static bool initialized;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <remarks>
        /// Gets called as part of the EPiServer Framework initialization sequence. Note that it will be called
        ///     only once per AppDomain, unless the method throws an exception. If an exception is thrown, the initialization
        ///     method will be called repeatedly for each request reaching the site until the method succeeds.
        /// </remarks>
        public void Initialize(InitializationEngine context)
        {
            if (initialized || context == null || context.HostType != HostType.WebApplication)
            {
                return;
            }

            Logger.Info("[Content] Initializing content providers.");

            // Initialize the provider after the initialization is complete.
            context.InitComplete += this.InitComplete;

            Event removeFromCacheEvent = Event.Get(CacheManager.RemoveFromCacheEventId);
            removeFromCacheEvent.Raised += RemoveFromCacheEventRaised;

            Logger.Info("[Content] Content providers initialized.");
        }

        /// <summary>
        /// Preloads the module.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <remarks>
        /// This method is only available to be compatible with "AlwaysRunning" applications in .NET 4 / IIS 7.
        ///     It currently serves no purpose.
        /// </remarks>
        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        /// Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method is usually not called when running under a web application since the web app may be shut down very
        ///         abruptly, but your module should still implement it properly since it will make integration and unit testing
        ///         much simpler.
        ///     </para>
        /// <para>
        /// Any work done by
        ///         <see cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)"/>
        ///         as well as any code executing on
        ///         <see cref="E:EPiServer.Framework.Initialization.InitializationEngine.InitComplete"/>
        ///         should be reversed.
        ///     </para>
        /// </remarks>
        public void Uninitialize(InitializationEngine context)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Run when initialization is complete.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        internal void InitComplete(object sender, EventArgs e)
        {
            initialized = LoadProviders();
        }

        /// <summary>
        ///     Loads the provider.
        /// </summary>
        /// <returns>
        ///     [true] if the provider has been loaded.
        /// </returns>
        private static bool LoadProviders()
        {
            IContentProviderManager providerManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();

            Collection<ClonedContentProviderSettings> providerCollection = SettingsRepository.Instance.GetAll();

            foreach (ClonedContentProviderSettings providerSettings in providerCollection)
            {
                try
                {
                    ContentProvider contentProvider = providerManager.GetProvider(providerSettings.Name);

                    if (contentProvider != null)
                    {
                        continue;
                    }

                    CategoryList categoryList =
                        new CategoryList(
                            providerSettings.CategoryList.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                            .Select(int.Parse)
                                            .ToArray());

                    ClonedContentProvider provider = new ClonedContentProvider(
                        providerSettings.Name, 
                        new PageReference(providerSettings.Root), 
                        new PageReference(providerSettings.EntryPoint), 
                        categoryList);

                    providerManager.ProviderMap.AddProvider(provider);
                }
                catch (ArgumentNullException)
                {
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Removes from cache event raised. 
        /// </summary>
        /// <param name="sender">
        /// Source of the event. 
        /// </param>
        /// <param name="e">
        /// Page event information. 
        /// </param>
        private static void RemoveFromCacheEventRaised(object sender, EventNotificationEventArgs e)
        {
            // We don't want to process events raised on this machine so we will check the raiser id.
            if (e.RaiserId != CacheManager.LocalCacheManagerRaiserId)
            {
                LoadProviders();
            }
        }

        #endregion
    }
}