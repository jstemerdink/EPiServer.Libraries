// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContextHelper.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Helpers
{
    using System;
    using System.Linq;
    using System.Web;

    using EPiServer.ServiceLocation;
    using EPiServer.Web;

    /// <summary>
    /// The context extensions.
    /// </summary>
    public static class ContextHelper
    {
        #region Public Methods and Operators

        /// <summary>
        /// Determines whether [is json accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is json accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsJsonAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return httpContext.Request.AcceptTypes != null
                   && httpContext.Request.AcceptTypes.Any(
                       t => t.Equals(OutputConstants.ApplicationJSON, StringComparison.OrdinalIgnoreCase));
        }

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
                ServiceLocator.Current.GetInstance<DisplayChannelService>()
                    .GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.JSON, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is PDF accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is PDF accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsPdfAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return httpContext.Request.AcceptTypes != null
                   && httpContext.Request.AcceptTypes.Any(
                       t => t.Equals(OutputConstants.ApplicationPDF, StringComparison.OrdinalIgnoreCase));
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
                ServiceLocator.Current.GetInstance<DisplayChannelService>()
                    .GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.PDF, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is TXT accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is TXT accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsTxtAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return httpContext.Request.AcceptTypes != null
                   && httpContext.Request.AcceptTypes.Any(
                       t => t.Equals(OutputConstants.TextPlain, StringComparison.OrdinalIgnoreCase));
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
                ServiceLocator.Current.GetInstance<DisplayChannelService>()
                    .GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.Text, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is XML accepted] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is XML accepted] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsXmlAccepted(this HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                return false;
            }

            return httpContext.Request.AcceptTypes != null
                   && httpContext.Request.AcceptTypes.Any(
                       t => t.Equals(OutputConstants.TextXML, StringComparison.OrdinalIgnoreCase));
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
                ServiceLocator.Current.GetInstance<DisplayChannelService>()
                    .GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.XML, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}