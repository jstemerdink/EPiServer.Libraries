// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextChannel.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Channels.Display
{
    using System.Web;

    using EPiServer.Web;

    /// <summary>
    /// The json channel.
    /// </summary>
    public class TextChannel : DisplayChannel
    {
        #region Public Properties

        /// <summary>
        /// Gets the channel name.
        /// </summary>
        public override string ChannelName
        {
            get
            {
                return OutputConstants.Text;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The is active.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsActive(HttpContextBase context)
        {
            if (context == null)
            {
                return false;
            }

            return ChannelSettings.Instance.EnableTXT && context.IsTxtAccepted();
        }

        #endregion
    }
}