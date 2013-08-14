// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputTemplatePage.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Channels
{
    using System;

    using EPiServer.Core;

    /// <summary>
    /// The output template page.
    /// </summary>
    /// <typeparam name="T">
    /// The generic type.
    /// </typeparam>
    public class OutputTemplatePage<T> : TemplatePage<T>
        where T : PageData
    {
        #region Methods

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            ChannelHelper.UseActiveChannel(this.CurrentPage);
        }

        #endregion
    }
}