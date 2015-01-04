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

using System.ComponentModel.DataAnnotations;

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Filters;

namespace EPiServer.Libraries.Localization.Models
{
    /// <summary>
    ///     A container to hold translations.
    /// </summary>
    /// <remarks>
    ///     To reference this use the name without spaces and special characters, all lower case.
    ///     So if the Name of the container is e.g. Jeroen Stemerdink, the key in the xml will be jeroenstemerdink.
    /// </remarks>
    /// <example>
    ///     <![CDATA[
    ///         <EPiServer:Translate runat="server" Text="/jeroenstemerdink/textone" />
    ///     ]]>
    /// </example>
    [ContentType(GUID = "{40393E0A-81EF-4B9A-B0AC-F883C036359D}", AvailableInEditMode = true,
        Description = "Container to hold translations", DisplayName = "Translation container",
        GroupName = "Localization")]
    [AvailableContentTypes(
        Include = new[] { typeof(TranslationItem), typeof(TranslationContainer), typeof(CategoryTranslationContainer) })
    ]
    public class TranslationContainer : PageData
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

        /// <summary>
        ///     Gets or sets the container name.
        /// </summary>
        /// <remarks>
        ///     To reference this use the text without spaces and special characters, all lower case.
        ///     So if the ContainerName is e.g. ContainerName, the key in the xml will be containername.
        /// </remarks>
        /// <example>
        ///     <![CDATA[
        ///         <EPiServer:Translate runat="server" Text="/containername/textone" />
        ///     ]]>
        /// </example>
        [Display(GroupName = SystemTabNames.Content, Description = "The name of the container.", Name = "Container name"
            )]
        [CultureSpecific(false)]
        [Required(AllowEmptyStrings = false)]
        public virtual string ContainerName
        {
            get
            {
                string containerName = this.GetPropertyValue(p => p.ContainerName);

                // Use explicitly set container name, otherwise fall back to page name
                return !string.IsNullOrWhiteSpace(containerName)
                       ? containerName
                       : PageName;
            }
            set { this.SetPropertyValue(p => p.ContainerName, value); }
            
        }
    }
}