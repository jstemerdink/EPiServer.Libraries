// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PdfOutputFormat.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Formats
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Web;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.DataAnnotations;
    using EPiServer.Libraries.Output.Helpers;
    using EPiServer.ServiceLocation;

    using iTextSharp.text;
    using iTextSharp.text.html.simpleparser;
    using iTextSharp.text.pdf;

    using log4net;

    /// <summary>The pdf output format.</summary>
    [OutputFormat(OutputConstants.PDF)]
    public class PdfOutputFormat : IOutputFormat
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="PdfOutputFormat" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(PdfOutputFormat));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Generates the PDF.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The pdf in bytes.
        /// </returns>
        public byte[] GeneratePdf(PageData page)
        {
            List<KeyValuePair<string, string>> propertyValues = page.GetPropertyValues();

            IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            PageData startPage = contentRepository.Get<PageData>(ContentReference.StartPage);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (Document document = new Document(PageSize.A4))
                {
                    PdfWriter.GetInstance(document, memoryStream);

                    document.AddCreationDate();

                    string author = page["Author"] as string;
                    string title = page["Title"] as string;
                    XhtmlString pdfHeader = startPage["PdfHeader"] as XhtmlString;
                    XhtmlString pdfFooter = startPage["PdfFooter"] as XhtmlString;

                    if (!string.IsNullOrWhiteSpace(author))
                    {
                        document.AddAuthor(author);
                    }

                    document.AddTitle(!string.IsNullOrWhiteSpace(title) ? title : page.Name);

                    document.Open();

                    Dictionary<string, object> providers = new Dictionary<string, object>
                                                               {
                                                                   {
                                                                       HTMLWorker.IMG_PROVIDER,
                                                                       new ImageProvider(
                                                                       document)
                                                                   }
                                                               };

                    StyleSheet styleSheet = OutputHelper.GetStyleSheet();

                    try
                    {
                        Paragraph paragraph = new Paragraph(!string.IsNullOrWhiteSpace(title) ? title : page.Name);
                        document.Add(paragraph);

                        // Add the header
                        if (pdfHeader != null && !pdfHeader.IsEmpty)
                        {
                            using (StringReader stringReader = new StringReader(pdfHeader.ToHtmlString()))
                            {
                                foreach (IElement element in
                                    HTMLWorker.ParseToList(stringReader, styleSheet, providers))
                                {
                                    document.Add(element);
                                }
                            }
                        }

                        // Add the selected properties
                        foreach (KeyValuePair<string, string> content in propertyValues)
                        {
                            using (StringReader stringReader = new StringReader(content.Value))
                            {
                                foreach (IElement element in
                                    HTMLWorker.ParseToList(stringReader, styleSheet, providers))
                                {
                                    document.Add(element);
                                }
                            }
                            
                            document.Add(new Chunk(Environment.NewLine));
                        }

                        // Add the footer
                        if (pdfFooter != null && !pdfFooter.IsEmpty)
                        {
                            using (StringReader stringReader = new StringReader(pdfFooter.ToHtmlString()))
                            {
                                foreach (IElement element in
                                    HTMLWorker.ParseToList(stringReader, styleSheet, providers))
                                {
                                    document.Add(element);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Paragraph paragraph = new Paragraph("Error!" + exception.Message);
                        Chunk text = paragraph.Chunks[0];
                        if (text != null)
                        {
                            text.Font.Color = BaseColor.RED;
                        }

                        document.Add(paragraph);
                    }

                    document.CloseDocument();
                }

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// The handle format.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public void HandleFormat(PageData page, HttpContextBase context)
        {
            if (!OutputHelper.IsValidRequest(page, context, Logger))
            {
                return;
            }

            string title = page["Title"] as string;

            string contentDisposition = string.Format(
                CultureInfo.InvariantCulture, 
                "attachment; filename={0}.pdf", 
                !string.IsNullOrWhiteSpace(title) ? title : page.Name);

            context.Response.Clear();

            context.Response.AddHeader("Content-Type", OutputConstants.ApplicationPDF);
            context.Response.AddHeader("Content-Disposition", contentDisposition);
            context.Response.BinaryWrite(this.GeneratePdf(page));
            context.Response.Flush();
            context.Response.End();
        }

        #endregion
    }
}