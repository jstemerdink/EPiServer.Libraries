// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonOutputFormat.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Formats
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Web;

    using EPiServer.Core;
    using EPiServer.Core.Html;
    using EPiServer.Libraries.Output.DataAnnotations;
    using EPiServer.Libraries.Output.Helpers;

    using log4net;

    using Newtonsoft.Json;

    /// <summary>The json output format.</summary>
    [OutputFormat(OutputConstants.JSON)]
    public class JsonOutputFormat : IOutputFormat
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="JsonOutputFormat" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(JsonOutputFormat));

        #endregion

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
        public void HandleFormat(PageData page, HttpContextBase context)
        {
            if (!OutputHelper.IsValidRequest(page, context, Logger))
            {
                return;
            }

            context.Response.Clear();

            context.Response.AddHeader("Content-Type", OutputConstants.ApplicationJSON);

            context.Response.Write(this.GenerateJson(page));
            context.Response.End();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generate the json output for the page.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GenerateJson(IContent page)
        {
            string json;

            List<KeyValuePair<string, string>> propertyValues = page.GetPropertyValues();

            StringBuilder stringBuilder = new StringBuilder();

            using (StringWriter sw = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            {
                JsonWriter jsonWriter = new JsonTextWriter(sw) { Formatting = Formatting.Indented };

                jsonWriter.WriteStartObject();

                foreach (KeyValuePair<string, string> content in propertyValues)
                {
                    jsonWriter.WritePropertyName(content.Key);
                    jsonWriter.WriteValue(TextIndexer.StripHtml(content.Value, content.Value.Length));
                }

                jsonWriter.WriteEndObject();

                json = sw.ToString();
            }

            return json;
        }

        #endregion
    }
}