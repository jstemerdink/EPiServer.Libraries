// Copyright© 2014 Jeroen Stemerdink. All Rights Reserved.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Specialized;
using System.Linq;

using EPiServer.Events;
using EPiServer.Events.Clients;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Framework.Localization;
using EPiServer.Libraries.Localization.Models;
using EPiServer.ServiceLocation;

using log4net;

namespace EPiServer.Libraries.Localization
{
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
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="TranslationProviderInitialization" />
        ///     class.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(TranslationProviderInitialization));

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

            Log.Info("[Localization] Initializing translation provider.");

            // Initialize the provider after the initialization is complete, else the StartPage cannot be found.
            context.InitComplete += this.InitComplete;

            // Attach events to update the translations when a translation or container is published, moved or deleted.
            DataFactory.Instance.PublishedPage += this.InstancePublishedPage;
            DataFactory.Instance.MovedPage += this.InstanceChangedPage;
            DataFactory.Instance.DeletedPage += this.InstanceChangedPage;

            initialized = true;

            Log.Info("[Localization] Translation provider initialized.");
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
        ///         <see
        ///             cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)" />
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

            Log.Info("[Localization] Uninitializing translation provider.");

            TranslationProvider translationProvider = GetTranslationProvider();

            UnLoadProvider(translationProvider);
            initialized = false;

            Log.Info("[Localization] Translation provider uninitialized.");
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
            initialized = LoadProvider();
        }

        /// <summary>
        ///     Gets the localization service.
        /// </summary>
        /// <returns>
        ///     The current <see cref="ProviderBasedLocalizationService" />.
        /// </returns>
        private static ProviderBasedLocalizationService GetLocalizationService()
        {
            try
            {
                // Casts the current LocalizationService to a ProviderBasedLocalizationService to get access to the current list of providers.
                return ServiceLocator.Current.GetInstance<LocalizationService>() as ProviderBasedLocalizationService;
            }
            catch (ActivationException)
            {
                return null;
            }
        }

        /// <summary>
        ///     Get the localization provider.
        /// </summary>
        /// <returns>
        ///     The <see cref="LocalizationProvider" />.
        /// </returns>
        private static TranslationProvider GetTranslationProvider()
        {
            ProviderBasedLocalizationService service = GetLocalizationService();
            if (service == null)
            {
                return null;
            }

            // Gets any provider that has the same name as the one initialized.
            LocalizationProvider localizationProvider =
                service.Providers.FirstOrDefault(
                    p => p.Name.Equals(ProviderName, StringComparison.Ordinal));

            return localizationProvider as TranslationProvider;
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

            TranslationFactory.Instance.LocalizationProvider = null;
        }

        private void InstancePublishedPage(object sender, PageEventArgs e)
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

            TranslationFactory.Instance.TranslateThemAll(e.Page);
            TranslationFactory.Instance.LocalizationProvider = null;
        }

        internal static LocalizationProvider InitializeProvider(LocalizationProvider provider)
        {
            // This config value could tell the provider where to find the translations, 
            // set to 0 though, will be looked up after initialization in the provider itself.
            NameValueCollection configValues = new NameValueCollection { { "containerid", "0" } };

            // Instantiate the provider
            provider.Initialize(ProviderName, configValues);

            return provider;
        }

        /// <summary>
        ///     Loads the provider.
        /// </summary>
        /// <returns>
        ///     [true] if the provider has been loaded.
        /// </returns>
        private static bool LoadProvider()
        {
            ProviderBasedLocalizationService service = GetLocalizationService();
            if (service == null)
            {
                return false;
            }

            LocalizationProvider translationProviderProvider = InitializeProvider(new TranslationProvider());

            // Add it at the end of the list of providers.
            try
            {
                service.Providers.Add(translationProviderProvider);
            }
            catch (NotSupportedException notSupportedException)
            {
                Log.Error("[Localization] Error add provider to the Localization Service.", notSupportedException);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Unloads the provider.
        /// </summary>
        private static void UnLoadProvider(LocalizationProvider localizationProvider)
        {
            ProviderBasedLocalizationService service = GetLocalizationService();
            if (service == null)
            {
                return;
            }

            if (localizationProvider != null)
            {
                // If found, remove it.
                service.Providers.Remove(localizationProvider);
            }
        }

        #endregion
    }
}