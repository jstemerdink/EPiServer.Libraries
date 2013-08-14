// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputSpecs.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.UnitTests.Specs
{
    using EPiServer.Core;
    using EPiServer.Libraries.Output;
    using EPiServer.Libraries.UnitTests.Models;

    using FakeItEasy;

    using Machine.Specifications;
    using Machine.Specifications.Annotations;

    /// <summary>
    ///     The translation specs.
    /// </summary>
    public abstract class OutputSpecs
    {
        #region Constants

        /// <summary>
        ///     The content not to display
        /// </summary>
        public const string ContentNotToDisplay = "Content not to display";

        /// <summary>
        ///     The content to display
        /// </summary>
        public const string ContentToDisplay = "Content to display";

        /// <summary>
        ///     The PDF footer
        /// </summary>
        public const string PdfFooter = "PDF Footer";

        /// <summary>
        ///     The PDF header
        /// </summary>
        public const string PdfHeader = "PDF Header";

        #endregion

        #region Fields

        /// <summary>
        ///     The context.
        /// </summary>
        [UsedImplicitly]
        private Establish context = () =>
            {
                CmsContext = new CmsContext();

                CmsContext.CreatePageType(typeof(OutputItem));

                PageData startPage = CreateStartPage("StartPage", PdfHeader, PdfFooter, ContentReference.RootPage);

                A.CallTo(() => CmsContext.ContentRepository.Get<PageData>(ContentReference.StartPage))
                    .Returns(startPage);

                OutputItem = CreateOutputItem(
                    "OutputTest", 
                    ContentToDisplay, 
                    ContentNotToDisplay, 
                    ContentReference.StartPage);

                NoOutputItem = CreateNoOutputItem(
                   "NoOutputTest",
                   ContentToDisplay,
                   ContentNotToDisplay,
                   ContentReference.StartPage);
            };

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the fake EPiServer context.
        /// </summary>
        [NotNull]
        public static CmsContext CmsContext { get; set; }

        /// <summary>
        /// Gets or sets the "no" output item.
        /// </summary>
        [NotNull]
        public static NoOutputItem NoOutputItem { get; set; }

        /// <summary>
        ///     Gets or sets the output item.
        /// </summary>
        [NotNull]
        public static OutputItem OutputItem { get; set; }

        /// <summary>
        ///     Gets or sets the output settings.
        /// </summary>
        [NotNull]
        public static OutputSettings OutputSettings { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates the "no" output item.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="textToUse">The text to use.</param>
        /// <param name="textNotToUse">The text not to use.</param>
        /// <param name="parentLink">The parent link.</param>
        /// <returns>
        /// The <see cref="NoOutputItem" />
        /// </returns>
        [NotNull]
        protected static NoOutputItem CreateNoOutputItem(
            [NotNull] string pageName,
            [NotNull] string textToUse,
            [NotNull] string textNotToUse,
            [NotNull] ContentReference parentLink)
        {
            // Create the base item
            NoOutputItem outputItem = CmsContext.CreateContent<NoOutputItem>(pageName, parentLink);

            // Set the additional properties for this type.
            outputItem.TextToUseInOutput = textToUse;
            outputItem.TextNotToUseInOutput = textNotToUse;

            return outputItem;
        }

        /// <summary>
        /// Creates the output item.
        /// </summary>
        /// <param name="pageName">Name of the page.</param>
        /// <param name="textToUse">The text to use.</param>
        /// <param name="textNotToUse">The text not to use.</param>
        /// <param name="parentLink">The parent link.</param>
        /// <returns>
        /// The <see cref="OutputItem" />
        /// </returns>
        [NotNull]
        protected static OutputItem CreateOutputItem(
            [NotNull] string pageName, 
            [NotNull] string textToUse, 
            [NotNull] string textNotToUse, 
            [NotNull] ContentReference parentLink)
        {
            // Create the base item
            OutputItem outputItem = CmsContext.CreateContent<OutputItem>(pageName, parentLink);

            // Set the additional properties for this type.
            outputItem.TextToUseInOutput = textToUse;
            outputItem.TextNotToUseInOutput = textNotToUse;

            return outputItem;
        }

        /// <summary>
        /// The create start page.
        /// </summary>
        /// <param name="pageName">The page name.</param>
        /// <param name="pdfHeader">The pdf header.</param>
        /// <param name="pdfFooter">The pdf footer.</param>
        /// <param name="parentLink">The parent link.</param>
        /// <returns>
        /// The <see cref="PageData" />.
        /// </returns>
        [NotNull]
        protected static PageData CreateStartPage(
            [NotNull] string pageName, 
            [NotNull] string pdfHeader, 
            [NotNull] string pdfFooter, 
            [NotNull] ContentReference parentLink)
        {
            // Create the base item
            PageData startPage = CmsContext.CreateContent<PageData>(pageName, parentLink);

            // Set the additional properties for this type.
            startPage.Property["PdfHeader"] = new PropertyString(pdfHeader);
            startPage.Property["PdfFooter"] = new PropertyString(pdfFooter);

            return startPage;
        }

        #endregion
    }
}