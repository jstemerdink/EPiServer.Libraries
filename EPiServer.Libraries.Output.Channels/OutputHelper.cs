// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputHelper.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Channels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.Formats;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;

    /// <summary>
    /// The output helper.
    /// </summary>
    public static class OutputHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Handles the channels.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="currentPage">The current Page.</param>
        public static void UseActiveChannel<T>(T currentPage) where T : PageData
        {
            List<DisplayChannel> channels =
                ServiceLocator.Current.GetInstance<DisplayChannelService>()
                    .GetActiveChannels(new HttpContextWrapper(HttpContext.Current))
                    .ToList();

            bool isJsonChannel =
                channels.Any(
                    c => string.Equals(c.ChannelName, OutputConstants.JSON, StringComparison.OrdinalIgnoreCase));

            bool isXmlChannel =
                channels.Any(c => string.Equals(c.ChannelName, OutputConstants.XML, StringComparison.OrdinalIgnoreCase));

            bool isTextChannel =
                channels.Any(
                    c => string.Equals(c.ChannelName, OutputConstants.Text, StringComparison.OrdinalIgnoreCase));

            bool isPdfChannel =
                channels.Any(c => string.Equals(c.ChannelName, OutputConstants.PDF, StringComparison.OrdinalIgnoreCase));

            if (isJsonChannel)
            {
                JsonOutputFormat jsonOutputFormat = new JsonOutputFormat();
                jsonOutputFormat.HandleFormat(currentPage, new HttpContextWrapper(HttpContext.Current));
            }

            if (isXmlChannel)
            {
                XmlOutputFormat xmlOutputFormat = new XmlOutputFormat();
                xmlOutputFormat.HandleFormat(currentPage, new HttpContextWrapper(HttpContext.Current));
            }

            if (isTextChannel)
            {
                TxtOutputFormat txtOutputFormat = new TxtOutputFormat();
                txtOutputFormat.HandleFormat(currentPage, new HttpContextWrapper(HttpContext.Current));
            }

            if (isPdfChannel)
            {
                PdfOutputFormat pdfOutputFormat = new PdfOutputFormat();
                pdfOutputFormat.HandleFormat(currentPage, new HttpContextWrapper(HttpContext.Current));
            }
        }

        #endregion
    }
}