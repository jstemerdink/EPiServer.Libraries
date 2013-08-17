// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelHelper.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Channels
{
    using System;
    using System.Linq;
    using System.Web;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.Helpers;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;

    /// <summary>
    /// The output helper.
    /// </summary>
    public static class ChannelHelper
    {
        /// <summary>
        /// Determines whether [is json display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is json display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJsonDisplayModeActive(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                ServiceLocator.Current.GetInstance<DisplayChannelService>().GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.JSON, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is json accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <c>true</c> if [is json accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJsonAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                httpContext.Request.AcceptTypes != null && httpContext.Request.AcceptTypes.Any(
                    t => t.Equals(OutputConstants.ApplicationJSON, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is PDF display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is PDF display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPdfDisplayModeActive(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                ServiceLocator.Current.GetInstance<DisplayChannelService>().GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.PDF, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is PDF accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <c>true</c> if [is PDF accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPdfAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                httpContext.Request.AcceptTypes != null && httpContext.Request.AcceptTypes.Any(
                    t => t.Equals(OutputConstants.ApplicationPDF, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is TXT display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is TXT display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTxtDisplayModeActive(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                ServiceLocator.Current.GetInstance<DisplayChannelService>().GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.Text, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is TXT accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <c>true</c> if [is TXT accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTxtAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                httpContext.Request.AcceptTypes != null && httpContext.Request.AcceptTypes.Any(
                    t => t.Equals(OutputConstants.TextPlain, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is XML display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is XML display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsXmlDisplayModeActive(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                ServiceLocator.Current.GetInstance<DisplayChannelService>().GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.XML, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is XML accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">The HTTP context.</param>
        /// <returns>
        ///   <c>true</c> if [is XML accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsXmlAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return
                httpContext.Request.AcceptTypes != null && httpContext.Request.AcceptTypes.Any(
                    t => t.Equals(OutputConstants.TextXML, StringComparison.OrdinalIgnoreCase));
        }

        #region Public Methods and Operators

        /// <summary>
        /// Handles the channels.
        /// </summary>
        /// <typeparam name="T">The generic type.</typeparam>
        /// <param name="currentPage">The current Page.</param>
        public static void UseActiveChannel<T>(T currentPage) where T : PageData
        {
            HttpContextWrapper httpContext = new HttpContextWrapper(HttpContext.Current);

            if (IsJsonDisplayModeActive(httpContext))
            {
                OutputHelper.HandleJson(currentPage);
            }

            if (IsXmlDisplayModeActive(httpContext))
            {
                OutputHelper.HandleXml(currentPage);
            }

            if (IsTxtDisplayModeActive(httpContext))
            {
                OutputHelper.HandleTxt(currentPage);
            }

            if (IsPdfDisplayModeActive(httpContext))
            {
                OutputHelper.HandlePdf(currentPage);
            }
        }

        #endregion
    }
}