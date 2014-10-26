// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputItem.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.UnitTests.Models
{
    using System.ComponentModel.DataAnnotations;

    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Libraries.Output.DataAnnotations;

    /// <summary>
    ///     An output test PageType.
    /// </summary>
    [ContentType(GUID = "{BE6C87B7-19D9-4C79-B089-AA6A1FB98F96}", AvailableInEditMode = true, 
        Description = "Output test item", DisplayName = "Output", GroupName = "Output")]
    [AvailableContentTypes(Exclude = new[] { typeof(PageData) })]
    public class OutputItem : PageData
    {
        #region Public Properties

        /// <summary>
        ///     Gets or sets the text not to use in output.
        /// </summary>
        /// <value>
        ///     The text not to use in output.
        /// </value>
        [Display(GroupName = SystemTabNames.Content, Description = "A text to use in a different output.", 
            Name = "Do not use in output")]
        [CultureSpecific]
        [Required(AllowEmptyStrings = false)]
        public virtual string TextNotToUseInOutput { get; set; }

        /// <summary>
        ///     Gets or sets the text to use in output.
        /// </summary>
        /// <value>
        ///     The text to use in output.
        /// </value>
        [Display(GroupName = SystemTabNames.Content, Description = "A text to use in a different output.", 
            Name = "Use in output")]
        [Required(AllowEmptyStrings = false)]
        [UseInOutput]
        public virtual string TextToUseInOutput { get; set; }

        #endregion
    }
}