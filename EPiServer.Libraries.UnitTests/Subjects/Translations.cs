// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Translations.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.UnitTests.Subjects
{
    using System.Threading;

    using EPiServer.DataAbstraction;
    using EPiServer.Framework.Localization;
    using EPiServer.Libraries.UnitTests.Specs;
    using EPiServer.Web.WebControls;

    using Machine.Specifications;

    /// <summary>
    /// Get a translation for a first level translation item.
    /// </summary>
    [Subject("Translations")]
    public class Get_a_translation_value_for_a_first_level_translation_item : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The of.
        /// </summary>
        private Because of = () => result = CmsContext.ProviderBasedLocalizationService.GetString("/textone");

        /// <summary>
        /// The should_be_translated.
        /// </summary>
        private It should_be_translated = () => result.ShouldEqual("Translation One");
    }

    /// <summary>
    /// Get a translation for a second language in a first level translation item.
    /// </summary>
    [Subject("Translations")]
    public class Get_a_Dutch_translation_value_for_a_first_level_translation_item : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The context
        /// </summary>
        private Establish context = () =>
        {
            Thread.CurrentThread.CurrentCulture = CmsContext.SecondLanguage;
            Thread.CurrentThread.CurrentUICulture = CmsContext.SecondLanguage;
        };

        /// <summary>
        /// The cleanup
        /// </summary>
        private Cleanup cleanup = () =>
        {
            Thread.CurrentThread.CurrentCulture = CmsContext.MasterLanguage;
            Thread.CurrentThread.CurrentUICulture = CmsContext.MasterLanguage;
        };

        /// <summary>
        /// The of.
        /// </summary>
        private Because of = () => result = CmsContext.ProviderBasedLocalizationService.GetString("/textone");

        /// <summary>
        /// The should_be_translated.
        /// </summary>
        private It should_be_translated_to = () => result.ShouldEqual("Vertaling Een");
    }

    /// <summary>
    /// Get a translation for a second level translation item.
    /// </summary>
    [Subject("Translations")]
    public class Get_a_translation_value_for_a_second_level_translation_item : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The of.
        /// </summary>
        private Because of = () => result = CmsContext.ProviderBasedLocalizationService.GetString("/subtranslations/subtextone");

        /// <summary>
        /// The should_be_translated.
        /// </summary>
        private It should_be_translated_to = () => result.ShouldEqual("SubTranslation One");
    }

    /// <summary>
    /// Get an empty string for a non existing translation item.
    /// </summary>
    [Subject("Translations")]
    public class Get_an_empty_value_for_a_non_existing_translation_item : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The of.
        /// </summary>
        private Because of = () => result = CmsContext.ProviderBasedLocalizationService.GetString("/texttwo");

        /// <summary>
        /// The should_not_be_translated.
        /// </summary>
        private It should_not_be_translated = () => result.ShouldEqual(string.Empty);
    }

    /// <summary>
    /// Get a missing message for a non existing translation item.
    /// </summary>
    [Subject("Translations")]
    public class Get_missing_message_for_a_non_existing_translation_item : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The of.
        /// </summary>
        private Because of = () => result = CmsContext.ProviderBasedLocalizationService.GetString("/texttwo", FallbackBehaviors.MissingMessage);

        /// <summary>
        /// Should return a missing message.
        /// </summary>
        private It should_return_the_missing_message = () => result.ShouldEqual("[Missing text '/texttwo' for 'English (United States)']");
    }

    /// <summary>
    /// Get a missing message for a non existing translation item.
    /// </summary>
    [Subject("Translations")]
    public class Get_echo_for_a_non_existing_translation_item : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The of.
        /// </summary>
        private Because of = () => result = CmsContext.ProviderBasedLocalizationService.GetString("texttwo", FallbackBehaviors.Echo);

        /// <summary>
        /// Should echo the original text.
        /// </summary>
        private It should_echo_the_original_text = () => result.ShouldEqual("texttwo");
    }

    /// <summary>
    /// Get a translation for a category.
    /// </summary>
    [Subject("Translations")]
    public class Get_a_translation_for_a_category : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The category
        /// </summary>
        private static Category category = new Category("CategoryOne", string.Empty);

        /// <summary>
        /// Should be the translated category.
        /// </summary>
        private Because of = () => result = category.LocalizedDescription;

        /// <summary>
        /// Should be the translated category.
        /// </summary>
        private It should_be_the_translated_category = () => result.ShouldEqual("CategoryTranslation One");
    }

    /// <summary>
    /// Get a Dutch translation for a category.
    /// </summary>
    [Subject("Translations")]
    public class Get_a_Dutch_translation_for_a_category : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The category
        /// </summary>
        private static Category category = new Category("CategoryOne", string.Empty);

        /// <summary>
        /// The context
        /// </summary>
        private Establish context = () =>
        {
            Thread.CurrentThread.CurrentCulture = CmsContext.SecondLanguage;
            Thread.CurrentThread.CurrentUICulture = CmsContext.SecondLanguage;
        };

        /// <summary>
        /// The cleanup
        /// </summary>
        private Cleanup cleanup = () =>
        {
            Thread.CurrentThread.CurrentCulture = CmsContext.MasterLanguage;
            Thread.CurrentThread.CurrentUICulture = CmsContext.MasterLanguage;
        };

        /// <summary>
        /// Should be the translated category.
        /// </summary>
        private Because of = () => result = category.LocalizedDescription;

        /// <summary>
        /// Should echo the original text.
        /// </summary>
        private It should_be_the_translated_category_for_Dutch = () => result.ShouldEqual("CategorieVertaling Een");
    }

    /// <summary>
    /// Get a translation with a translation control.
    /// </summary>
    [Subject("Translations")]
    public class Get_a_translation_for_a_translate_control : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The category
        /// </summary>
        private static Translate translate = new Translate { Text = "/textone" };

        /// <summary>
        /// Render the control.
        /// </summary>
        private Because of = () => result = CmsContext.GetRenderedText(translate);

        /// <summary>
        /// Should be the translated control.
        /// </summary>
        private It should_be_the_translated_value = () => result.ShouldEqual("Translation One");
    }

    /// <summary>
    /// Get a Durch translation with a translation control.
    /// </summary>
    [Subject("Translations")]
    public class Get_a_Dutch_translation_for_a_translate_control : TranslationSpecs
    {
        /// <summary>
        /// The result.
        /// </summary>
        private static string result;

        /// <summary>
        /// The control
        /// </summary>
        private static Translate translate = new Translate { Text = "/textone" };

        /// <summary>
        /// The context
        /// </summary>
        private Establish context = () =>
        {
            Thread.CurrentThread.CurrentCulture = CmsContext.SecondLanguage;
            Thread.CurrentThread.CurrentUICulture = CmsContext.SecondLanguage;
        };

        /// <summary>
        /// The cleanup
        /// </summary>
        private Cleanup cleanup = () =>
        {
            Thread.CurrentThread.CurrentCulture = CmsContext.MasterLanguage;
            Thread.CurrentThread.CurrentUICulture = CmsContext.MasterLanguage;
        };

        /// <summary>
        /// Render the control.
        /// </summary>
        private Because of = () => result = CmsContext.GetRenderedText(translate);

        /// <summary>
        /// Should be the translated control.
        /// </summary>
        private It should_be_the_translated_value = () => result.ShouldEqual("Vertaling Een");
    }
}