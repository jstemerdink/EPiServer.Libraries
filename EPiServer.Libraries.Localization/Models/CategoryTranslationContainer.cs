// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CategoryTranslationContainer.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Localization.Models
{
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Filters;

    /// <summary>
    ///     A container to hold translations for categories.
    /// </summary>
    /// <remarks>
    ///     Only the first container will be used in the xml, subs will only be used for ordering.
    /// </remarks>
    [ContentType(GUID = "{F95F6943-4ED8-4080-A3C1-D1E903512DB0}", AvailableInEditMode = true, Description = "Container to hold translations for categories", DisplayName = "Categorie translation container", GroupName = "Localization")]
    [AvailablePageTypes(Include = new[] { typeof(TranslationItem), typeof(CategoryTranslationContainer) })]
    public class CategoryTranslationContainer : PageData
    {
        /// <summary>
        /// Sets the default property values on the page data.
        /// </summary>
        /// <param name="contentType">The type of content.</param>
        /// <example>
        ///   <code source="../CodeSamples/EPiServer/Core/PageDataSamples.aspx.cs" region="DefaultValues" />
        /// </example>
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            this[MetaDataProperties.PageChildOrderRule] = FilterSortOrder.Alphabetical;
            this.VisibleInMenu = false;
        }
    }
}