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

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Filters;

namespace EPiServer.Libraries.Localization.Models
{
    /// <summary>
    ///     A container to hold translations for categories.
    /// </summary>
    /// <remarks>
    ///     Only the first container will be used in the xml, subs will only be used for ordering.
    /// </remarks>
    [ContentType(GUID = "{F95F6943-4ED8-4080-A3C1-D1E903512DB0}", AvailableInEditMode = true,
        Description = "Container to hold translations for categories", DisplayName = "Categorie translation container",
        GroupName = "Localization")]
    [AvailableContentTypes(Include = new[] { typeof(TranslationItem), typeof(CategoryTranslationContainer) })]
    public class CategoryTranslationContainer : PageData
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Sets the default property values on the page data.
        /// </summary>
        /// <param name="contentType">The type of content.</param>
        /// <example>
        ///     <code source="../CodeSamples/EPiServer/Core/PageDataSamples.aspx.cs" region="DefaultValues" />
        /// </example>
        public override void SetDefaultValues(ContentType contentType)
        {
            base.SetDefaultValues(contentType);
            this[MetaDataProperties.PageChildOrderRule] = FilterSortOrder.Alphabetical;
            this.VisibleInMenu = false;
        }

        #endregion
    }
}