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

namespace EPi.Libraries.SEO.Alchemy.Models
{
    /// <summary>
    ///     Class KeywordGenerationSettingsBlock.
    /// </summary>
    [ContentType(DisplayName = "Keyword Generation Settings", GUID = "32682699-7888-4002-93df-33dfb8b2745a",
        Description = "Settings for the keyword generation api of Alchemy", AvailableInEditMode = false)]
    public class KeywordGenerationSettingsBlock : BlockData
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the alchemy key.
        /// </summary>
        /// <value>The alchemy key.</value>
        [CultureSpecific(false)]
        [Required(AllowEmptyStrings = false)]
        [Display(Name = "Alchemy key", Description = "The API key for Alchemy", GroupName = SystemTabNames.Settings,
            Order = 1)]
        public virtual string AlchemyKey { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of keywords to retrieve.
        /// </summary>
        /// <value>The maximum maximum amount of keywords to retrieve.</value>
        [CultureSpecific(false)]
        [Required]
        [Range(5, 50)]
        [Display(Name = "Maximum amount", Description = "The maximum amount of keywords to retrieve.",
            GroupName = SystemTabNames.Settings, Order = 2)]
        public virtual int MaxItems { get; set; }

        #endregion
    }
}