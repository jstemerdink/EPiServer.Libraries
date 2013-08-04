// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputPartialRouter.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   The output partial router.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output
{
    using System;
    using System.Globalization;
    using System.Web;
    using System.Web.Routing;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.Formats;
    using EPiServer.Web.Routing;
    using EPiServer.Web.Routing.Segments;

    /// <summary>
    ///     The output partial router.
    /// </summary>
    public class OutputPartialRouter : IPartialRouter<PageData, string>
    {
        #region Public Methods and Operators

        /// <summary>
        /// Gets the partial virtual path.
        /// </summary>
        /// <param name="content">
        /// The output.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="routeValues">
        /// The route values.
        /// </param>
        /// <param name="requestContext">
        /// The request context.
        /// </param>
        /// <returns>
        /// The <see cref="PartialRouteData"/>.
        /// </returns>
        public PartialRouteData GetPartialVirtualPath(
            string content, string language, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            return new PartialRouteData
                       {
                           BasePathRoot = requestContext.GetContentLink(), 
                           PartialVirtualPath =
                               string.Format(CultureInfo.InvariantCulture, "{0}/", content)
                       };
        }

        /// <summary>
        /// The route partial.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="segmentContext">
        /// The segment context.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object RoutePartial(PageData content, SegmentContext segmentContext)
        {
            if (segmentContext == null)
            {
                return content;
            }

            // Use helper method GetNextValue to get the next part from the URL
            SegmentPair nextSegment = segmentContext.GetNextValue(segmentContext.RemainingPath);

            string output = nextSegment.Next;

            if (string.IsNullOrWhiteSpace(output))
            {
                return content;
            }

            segmentContext.RemainingPath = nextSegment.Remaining;

            if (output.Equals(OutputConstants.PDF, StringComparison.OrdinalIgnoreCase) && OutputSettings.Instance.EnablePDF)
            {
                PdfOutputFormat pdfOutputFormat = new PdfOutputFormat();
                pdfOutputFormat.HandleFormat(content, new HttpContextWrapper(HttpContext.Current)); 
            }

            if (output.Equals(OutputConstants.Text, StringComparison.OrdinalIgnoreCase) && OutputSettings.Instance.EnableTXT)
            {
                TxtOutputFormat txtOutputFormat = new TxtOutputFormat();
                txtOutputFormat.HandleFormat(content, new HttpContextWrapper(HttpContext.Current));
            }

            if (output.Equals(OutputConstants.XML, StringComparison.OrdinalIgnoreCase) && OutputSettings.Instance.EnableXML)
            {
                XmlOutputFormat xmlOutputFormat = new XmlOutputFormat();
                xmlOutputFormat.HandleFormat(content, new HttpContextWrapper(HttpContext.Current));
            }

            if (output.Equals(OutputConstants.JSON, StringComparison.OrdinalIgnoreCase) && OutputSettings.Instance.EnableJSON)
            {
                JsonOutputFormat jsonOutputFormat = new JsonOutputFormat();
                jsonOutputFormat.HandleFormat(content, new HttpContextWrapper(HttpContext.Current));
            }

            return content;
        }

        #endregion
    }
}