// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlOutputFormat.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Formats
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;
    using System.Xml;
    using System.Xml.Linq;

    using EPiServer.Core;
    using EPiServer.Core.Html;
    using EPiServer.Libraries.Output.DataAnnotations;
    using EPiServer.Libraries.Output.Helpers;

    using log4net;

    /// <summary>The xml output format.</summary>
    [OutputFormat(OutputConstants.XML)]
    public class XmlOutputFormat : IOutputFormat
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="XmlOutputFormat" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(XmlOutputFormat));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The generate xml.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GenerateXml(IContent page)
        {
            if (page == null)
            {
                return string.Empty;
            }

            XDocument xDocument = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new object[0]);

            XElement xElement = new XElement("content");
            xElement.SetAttributeValue("name", XmlConvert.EncodeName(page.Name));

            List<KeyValuePair<string, string>> propertyValues = page.GetPropertyValues();

            foreach (KeyValuePair<string, string> content in propertyValues)
            {
                XElement xElement3 = new XElement(XmlConvert.EncodeName(content.Key));
                xElement3.SetValue(TextIndexer.StripHtml(content.Value, content.Value.Length));
                xElement.Add(xElement3);
            }

            xDocument.Add(xElement);

            return xDocument.ToString();
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

            context.Response.Clear();

            context.Response.AddHeader("Content-Type", OutputConstants.TextXML);
            context.Response.ContentEncoding = new UTF8Encoding();

            context.Response.Write(GenerateXml(page));

            context.Response.End();
        }

        #endregion
    }
}