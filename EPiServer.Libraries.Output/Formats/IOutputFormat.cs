// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOutputFormat.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   The OutputFormat interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Formats
{
    using System.Web;

    using EPiServer.Core;

    /// <summary>The OutputFormat interface.</summary>
    public interface IOutputFormat
    {
        #region Public Methods and Operators

        /// <summary>
        /// The handle format.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        void HandleFormat(PageData page, HttpContextBase context);

        #endregion
    }
}