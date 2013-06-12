// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationProviderInitialization.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Localization
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;

    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Framework.Localization;
    using EPiServer.Libraries.Localization.Models;
    using EPiServer.ServiceLocation;

    using log4net;

    /// <summary>
    ///     The initialization module for the translation provider.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class TranslationProviderInitialization : IInitializableModule
    {
        #region Constants

        /// <summary>
        ///     The provider name.
        /// </summary>
        private const string ProviderName = "Translations";

        #endregion

        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="TranslationProviderInitialization" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TranslationProviderInitialization));

        /// <summary>
        ///     Check if the initialization has been done.
        /// </summary>
        private static bool initialized;

        #endregion

        #region Fields

        /// <summary>
        ///     The localization service
        /// </summary>
        private ProviderBasedLocalizationService localizationService;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the localization service.
        /// </summary>
        /// <value>
        ///     The localization service.
        /// </value>
        private ProviderBasedLocalizationService LocalizationService
        {
            get
            {
                return this.localizationService ?? (this.localizationService = GetLocalizationService());
            }
        }

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
            // If there is no context, we can't do anything.
            if (context == null)
            {
                return;
            }

            // If already initialized, no need to do it again.
            if (initialized)
            {
                return;
            }

            Logger.Info("[Localization] Initializing translation provider.");

            // Initialize the provider after the initialization is complete, else the StartPage cannot be found.
            context.InitComplete += this.InitComplete;

            // Attach events to update the translations when a translation or container is published, moved or deleted.
            DataFactory.Instance.PublishedPage += this.InstanceChangedPage;
            DataFactory.Instance.MovedPage += this.InstanceChangedPage;
            DataFactory.Instance.DeletedPage += this.InstanceChangedPage;

            initialized = true;

            Logger.Info("[Localization] Translation provider initialized.");
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
            // If there is no context, we can't do anything.
            if (context == null)
            {
                return;
            }

            // If already uninitialized, no need to do it again.
            if (!initialized)
            {
                return;
            }

            Logger.Info("[Localization] Uninitializing translation provider.");

            initialized = !this.UnLoadProvider();

            Logger.Info("[Localization] Translation provider uninitialized.");
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
            initialized = this.LoadProvider();
        }

        /// <summary>
        ///     Gets the localization service.
        /// </summary>
        /// <returns>
        ///     The current <see cref="ProviderBasedLocalizationService" />.
        /// </returns>
        private static ProviderBasedLocalizationService GetLocalizationService()
        {
            ProviderBasedLocalizationService service;

            try
            {
                // Casts the current LocalizationService to a ProviderBasedLocalizationService to get access to the current list of providers.
                service = ServiceLocator.Current.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            }
            catch (ActivationException)
            {
                return null;
            }

            return service;
        }

        /// <summary>
        /// Load the translations.
        /// </summary>
        /// <param name="localizationProvider">
        /// The localization Provider.
        /// </param>
        private static void LoadTranslations(TranslationProvider localizationProvider)
        {
            if (localizationProvider != null)
            {
                localizationProvider.LoadTranslations();
                return;
            }

            Logger.Info("[Localization] Translation provider not found, no translations loaded.");
        }

        /// <summary>
        ///     Get the localization provider.
        /// </summary>
        /// <returns>
        ///     The <see cref="LocalizationProvider" />.
        /// </returns>
        private LocalizationProvider GetLocalizationProvider()
        {
            if (this.LocalizationService == null)
            {
                return null;
            }

            // Gets any provider that has the same name as the one initialized.
            LocalizationProvider localizationProvider =
                this.LocalizationService.Providers.FirstOrDefault(
                    p => p.Name.Equals(ProviderName, StringComparison.Ordinal));

            return localizationProvider;
        }

        /// <summary>
        /// If a translation gets published, moved or deleted, update the provider.
        /// </summary>
        /// <param name="sender">
        /// Source of the event.
        /// </param>
        /// <param name="e">
        /// Page event information.
        /// </param>
        private void InstanceChangedPage(object sender, PageEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (!(e.Page is TranslationContainer) && !(e.Page is TranslationItem)
                && !(e.Page is CategoryTranslationContainer))
            {
                return;
            }

            this.ReloadProvider();
        }

        /// <summary>
        ///     Loads the provider.
        /// </summary>
        /// <returns>
        ///     [true] if the provider has been loaded.
        /// </returns>
        private bool LoadProvider()
        {
            if (this.LocalizationService == null)
            {
                return false;
            }

            // This config value could tell the provider where to find the translations, 
            // set to 0 though, will looked up after initialization in the provider itself.
            NameValueCollection configValues = new NameValueCollection { { "containerid", "0" } };

            TranslationProvider localizationProvider = new TranslationProvider();

            // Instanciate the provider
            localizationProvider.Initialize(ProviderName, configValues);

            // Add it at the end of the list of providers.
            this.LocalizationService.Providers.Add(localizationProvider);

            LoadTranslations(localizationProvider);

            return true;
        }

        /// <summary>
        ///     Reloads the provider.
        /// </summary>
        private void ReloadProvider()
        {
            initialized = this.UnLoadProvider();
            initialized = this.LoadProvider();
        }

        /// <summary>
        ///     Uns the load provider.
        /// </summary>
        /// <returns>
        ///     [true] if the provider has been unloaded.
        /// </returns>
        private bool UnLoadProvider()
        {
            if (this.LocalizationService == null)
            {
                return false;
            }

            LocalizationProvider localizationProvider = this.GetLocalizationProvider();

            if (localizationProvider != null)
            {
                // If found, remove it.
                this.LocalizationService.Providers.Remove(localizationProvider);
            }

            return true;
        }

        #endregion
    }
}