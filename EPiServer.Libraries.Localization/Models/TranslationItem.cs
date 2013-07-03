// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationItem.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Localization.Models
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;

    /// <summary>
    ///     The translation PageType.
    /// </summary>
    [ContentType(GUID = "{A691F851-6C6E-4C06-B62E-8FBC5A038A68}", AvailableInEditMode = true, 
        Description = "Translation", DisplayName = "Translation", GroupName = "Localization")]
    [AvailablePageTypes(Exclude = new[] { typeof(PageData) })]
    public class TranslationItem : PageData
    {
        #region Public Properties

        /// <summary>
        ///     Gets the lookup key this item.
        /// </summary>
        /// <remarks>
        /// Only for normal translation items. Displays the path to use in e.g. the translate control.
        ///     <![CDATA[
        ///         <EPiServer:Translate runat="server" Text="/jeroenstemerdink/textone" />
        ///     ]]> 
        /// </remarks>
        public string LookupKey
        {
            get
            {
                // If this is a category translation, no need to calculate a path.
                if (DataFactory.Instance.GetPage(this.ParentLink) is CategoryTranslationContainer)
                {
                    return "-";
                }

                // Use the masterlanguage branch, that one is always available.
                PageData masterLanguagePage = DataFactory.Instance.GetPage(
                    this.PageLink, new LanguageSelector(this.MasterLanguageBranch));

                // Get the ancestors
                IEnumerable<IContent> ancestors =
                    DataFactory.Instance.GetAncestors(masterLanguagePage.PageLink).Reverse();

                // Get all translation containers, skip the main one.
                List<string> keyParts =
                    ancestors.OfType<TranslationContainer>()
                             .Select(
                                 ancestor =>
                                 Regex.Replace(ancestor.Name.ToLowerInvariant(), @"[^A-Za-z0-9]+", string.Empty))
                             .Skip(1)
                             .ToList();

                // Add this file
                keyParts.Add(Regex.Replace(this.OriginalText.ToLowerInvariant(), @"[^A-Za-z0-9]+", string.Empty));

                return string.Format(CultureInfo.InvariantCulture, "/{0}", string.Join("/", keyParts));
            }
        }

        /// <summary>
        ///     Gets the missing translations for this item.
        /// </summary>
        public ReadOnlyCollection<string> MissingValues
        {
            get
            {
                IEnumerable<CultureInfo> availableLanguages =
                    DataFactory.Instance.GetLanguageBranches(ContentReference.StartPage)
                               .Select(pageData => pageData.Language)
                               .ToList();

                PageDataCollection allLanguages = DataFactory.Instance.GetLanguageBranches(this.PageLink);

                return new ReadOnlyCollection<string>(
                    (from availableLanguage in availableLanguages
                     where allLanguages.FirstOrDefault(p => p.Language.Equals(availableLanguage)) == null
                     select availableLanguage.NativeName).ToList());
            }
        }

        /// <summary>
        ///     Gets or sets the original text.
        /// </summary>
        /// <remarks>
        ///     To reference this use the text without spaces and special characters, all lower case.
        ///     So if the OriginalText is e.g. Text One, the key in the xml will be textone.
        ///     Note that this is only for normal translations. For translations beneath a
        ///     <see
        ///         cref="CategoryTranslationContainer" />
        ///     the OriginalText text will be used, as the generated xml is different for Category translations.
        ///     You can display the translated category name on a page by using the LocalizedDescription property of the Category.
        /// </remarks>
        /// <example>
        ///     <![CDATA[
        ///         <EPiServer:Translate runat="server" Text="/jeroenstemerdink/textone" />
        ///     ]]>
        /// </example>
        [Display(GroupName = SystemTabNames.Content, Description = "The text to translate.", Name = "Original text")]
        [Required(AllowEmptyStrings = false)]
        public virtual string OriginalText { get; set; }

        /// <summary>
        ///     Gets the translated values for this item.
        /// </summary>
        public Dictionary<string, string> TranslatedValues
        {
            get
            {
                PageDataCollection allLanguages = DataFactory.Instance.GetLanguageBranches(this.PageLink);

                return
                    new Dictionary<string, string>(
                        allLanguages.ToDictionary(
                            language => new CultureInfo(language.LanguageID).NativeName, 
                            language => ((TranslationItem)language).Translation));
            }
        }

        /// <summary>
        ///     Gets or sets the translation.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Description = "The translation of the original text.", 
            Name = "Translation")]
        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        public virtual string Translation { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Sets the default property values on the page data.
        /// </summary>
        /// <param name="contentType">
        /// The type of content.
        /// </param>
        /// <example>
        /// <code source="../CodeSamples/EPiServer/Core/PageDataSamples.aspx.cs" region="DefaultValues">
        /// </code>
        /// </example>
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);

            this.VisibleInMenu = false;
        }

        #endregion
    }
}