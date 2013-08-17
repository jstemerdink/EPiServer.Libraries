// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TxtOutputFormat.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Formats
{
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    using EPiServer.Core;
    using EPiServer.Core.Html;
    using EPiServer.Libraries.Output.DataAnnotations;
    using EPiServer.Libraries.Output.Helpers;

    using log4net;

    /// <summary>The txt output format.</summary>
    [OutputFormat(OutputConstants.Text)]
    public class TxtOutputFormat : IOutputFormat
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="TxtOutputFormat" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TxtOutputFormat));

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Generates the TXT.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// The <see cref="string" />.
        /// </returns>
        public static string GenerateTxt(IContent page)
        {
            if (page == null)
            {
                return string.Empty;
            }

            StringBuilder contentString = new StringBuilder();

            contentString.AppendLine(page.Name);
            contentString.AppendLine();

            List<KeyValuePair<string, string>> propertyValues = page.GetPropertyValues();

            foreach (KeyValuePair<string, string> content in propertyValues)
            {
                contentString.AppendLine(TextIndexer.StripHtml(content.Value, content.Value.Length));
                contentString.AppendLine();
            }

            return contentString.ToString();
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
            context.Response.AddHeader("Content-Type", OutputConstants.TextPlain);
            
            context.Response.Write(GenerateTxt(page));

            context.Response.End();
        }
        
        #endregion
    }
}