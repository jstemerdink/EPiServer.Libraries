// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategorieTranslationItem.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Localization.Models
{
    using System.ComponentModel.DataAnnotations;

    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;

    /// <summary>
    ///     The translation base.
    /// </summary>
    [ContentType(GUID = "{112A724E-200F-4C97-9D91-E06A28D18800}", AvailableInEditMode = true, 
        Description = "A Categorie translation", DisplayName = "Categorie translation", GroupName = "Localization")]
    [AvailablePageTypes(Exclude = new[] { typeof(PageData) })]
    public class CategorieTranslationItem : PageData
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the original text.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Description = "The text to translate.", Name = "Original text")]
        [Required(AllowEmptyStrings = false)]
        public virtual string OriginalText { get; set; }

        /// <summary>
        ///     Gets or sets the translation.
        /// </summary>
        [Display(GroupName = SystemTabNames.Content, Description = "The translation of the original text.", 
            Name = "Translation")]
        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        public virtual string Translation { get; set; }

        #endregion
    }
}