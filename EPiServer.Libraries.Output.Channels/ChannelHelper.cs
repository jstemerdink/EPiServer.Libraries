// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelHelper.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Channels
{
    using System.Web;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.Helpers;

    /// <summary>
    ///     The output helper.
    /// </summary>
    public static class ChannelHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Handles the channels.
        /// </summary>
        /// <typeparam name="T">
        /// The generic type.
        /// </typeparam>
        /// <param name="currentPage">
        /// The current Page.
        /// </param>
        public static void UseActiveChannel<T>(T currentPage) where T : PageData
        {
            HttpContextWrapper httpContext = new HttpContextWrapper(HttpContext.Current);

            if (httpContext.IsJsonDisplayModeActive())
            {
                OutputHelper.HandleJson(currentPage);
            }

            if (httpContext.IsXmlDisplayModeActive())
            {
                OutputHelper.HandleXml(currentPage);
            }

            if (httpContext.IsTxtDisplayModeActive())
            {
                OutputHelper.HandleTxt(currentPage);
            }

            if (httpContext.IsPdfDisplayModeActive())
            {
                OutputHelper.HandlePdf(currentPage);
            }
        }

        #endregion
    }
}