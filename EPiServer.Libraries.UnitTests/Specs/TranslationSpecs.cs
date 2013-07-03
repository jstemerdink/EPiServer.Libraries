// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationSpecs.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.UnitTests.Specs
{
    using System.Collections.Specialized;

    using EPiServer.Core;
    using EPiServer.Libraries.Localization;
    using EPiServer.Libraries.Localization.Models;

    using Machine.Specifications;
    using Machine.Specifications.Annotations;

    /// <summary>
    ///     The translation specs.
    /// </summary>
    public abstract class TranslationSpecs
    {
        #region Static Fields

        #endregion

        #region Fields

        /// <summary>
        ///     The context.
        /// </summary>
        [UsedImplicitly]
        private Establish context = () =>
            {
                CmsContext = new CmsContext();

                CmsContext.CreatePageType(typeof(TranslationContainer));
                CmsContext.CreatePageType(typeof(TranslationItem));
                CmsContext.CreatePageType(typeof(CategoryTranslationContainer));

                LanguageSelector masterLanguageSelector = new LanguageSelector(CmsContext.MasterLanguage.Name);
                LanguageSelector secondLanguageSelector = new LanguageSelector(CmsContext.SecondLanguage.Name);

                CmsContext.CreateContent<PageData>("StartPage", ContentReference.RootPage);

                ContentReference containerReference = CmsContext.CreateContent<TranslationContainer>("Translations", ContentReference.StartPage).ContentLink;
                CmsContext.CreateLanguageVersionOfContent<TranslationContainer>(containerReference, secondLanguageSelector);

                ContentReference itemReference = CreateTranslationItem("TextOne", "Translation One", containerReference, masterLanguageSelector).ContentLink;
                AddLanguageVersionToTranslationItem(itemReference, "Vertaling Een", secondLanguageSelector);

                ContentReference subContainerReference = CmsContext.CreateContent<TranslationContainer>("SubTranslations", containerReference).ContentLink;

                CreateTranslationItem("SubTextOne", "SubTranslation One", subContainerReference, masterLanguageSelector);

                ContentReference categoryContainerReference = CmsContext.CreateContent<CategoryTranslationContainer>("Categories", containerReference).ContentLink;
                CmsContext.CreateLanguageVersionOfContent<CategoryTranslationContainer>(categoryContainerReference, secondLanguageSelector);

                ContentReference categoryReference = CreateTranslationItem("CategoryOne", "CategoryTranslation One", categoryContainerReference, masterLanguageSelector).ContentLink;
                AddLanguageVersionToTranslationItem(categoryReference, "CategorieVertaling Een", secondLanguageSelector);

                NameValueCollection configValues = new NameValueCollection { { "containerid", "0" } };

                LocalizationProvider = new TranslationProvider();

                // Instanciate the provider
                LocalizationProvider.Initialize("Translations", configValues);

                // Add it at the end of the list of providers.
                CmsContext.ProviderBasedLocalizationService.Providers.Add(LocalizationProvider);
            };

        /// <summary>
        /// Gets or sets the fake EPiServer context.
        /// </summary>
        [NotNull]
        public static CmsContext CmsContext { get; set; }

        /// <summary>
        /// Gets or sets localization provider.
        /// </summary>
        [NotNull]
        public static TranslationProvider LocalizationProvider { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The create translation item.
        /// </summary>
        /// <param name="originalText">The original text.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="parentLink">The parent link.</param>
        /// <param name="languageSelector">The language selector.</param>
        /// <returns>The <see cref="ContentReference"/>.</returns>
        [NotNull]
        protected static TranslationItem CreateTranslationItem([NotNull] string originalText, [NotNull] string translation, [NotNull] ContentReference parentLink, [NotNull] LanguageSelector languageSelector)
        {
            // Create the base item
            TranslationItem translationItem = CmsContext.CreateContent<TranslationItem>(
                originalText, parentLink);

            // Set the additional properties for this type.
            translationItem.OriginalText = originalText;
            translationItem.Translation = translation;

            return translationItem;
        }

        /// <summary>
        /// Adds the language version to translation item.
        /// </summary>
        /// <param name="contentLink">The content link.</param>
        /// <param name="translation">The translation.</param>
        /// <param name="languageSelector">The language selector.</param>
        /// <returns>The <see cref="ContentReference"/>.</returns>
        /// <exception cref="EPiServer.Core.ContentNotFoundException">TranslationItem not found.</exception>
        /// <exception cref="EPiServer.Core.EPiServerException">Creating a copy of the item did not succeed.</exception>
        [NotNull]
        protected static TranslationItem AddLanguageVersionToTranslationItem([NotNull] ContentReference contentLink, [NotNull] string translation, [NotNull] LanguageSelector languageSelector)
        {
            // Create the base language version
            TranslationItem translationItem = CmsContext.CreateLanguageVersionOfContent<TranslationItem>(
                contentLink, languageSelector);

            // Change the properties that need changing for this version.
            translationItem.Translation = translation;

            return translationItem;
        }

        #endregion
    }
}