using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;

using EPiServer.Core;
using EPiServer.Libraries.SEO.Alchemy.Models;
using EPiServer.Libraries.SEO.DataAnnotations;
using EPiServer.ServiceLocation;

using log4net;
using log4net.Repository.Hierarchy;

using Newtonsoft.Json;

namespace EPiServer.Libraries.SEO.Alchemy
{
    /// <summary>
    /// Class AlchemyExtractionService.
    /// </summary>
    [ServiceConfiguration(typeof(IExtractionService))]
    public class AlchemyExtractionService : IExtractionService
    {
        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AlchemyExtractionService));

        /// <summary>
        /// Gets or sets the content repository.
        /// </summary>
        /// <value>The content repository.</value>
        protected Injected<IContentRepository> ContentRepository { get; set; }

        /// <summary>
        /// Gets or sets the maximum items.
        /// </summary>
        /// <value>The maximum items.</value>
        private int MaxItems { get; set; }

        /// <summary>
        /// Gets or sets the alchemy key.
        /// </summary>
        /// <value>The alchemy key.</value>
        private string AlchemyKey { get; set; }

        /// <summary>
        /// Gets the keywords.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>ReadOnlyCollection&lt;System.String&gt;.</returns>
        public ReadOnlyCollection<string> GetKeywords(string text)
        {
            this.GetSettings();

            if (string.IsNullOrWhiteSpace(this.AlchemyKey))
            {
                return new ReadOnlyCollection<string>(new List<string>());
            }
            try
            {
                string uri = string.Format(
                    CultureInfo.InvariantCulture,
                    "http://access.alchemyapi.com/calls/text/TextGetRankedKeywords?apikey={0}&text={1}&maxRetrieve={2}&keywordExtractMode=strict&outputMode=json",
                    this.AlchemyKey,
                    HttpUtility.UrlEncode(text),
                    this.MaxItems);

                WebRequest translationWebRequest = WebRequest.Create(uri);
                translationWebRequest.Method = "POST";

                WebResponse response = translationWebRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.GetEncoding("utf-8");

                if (stream == null)
                {
                    return null;
                }

                StreamReader translatedStream = new StreamReader(stream, encode);
                string json = translatedStream.ReadToEnd();

                AlchemyResponse alchemyResponse = JsonConvert.DeserializeObject<AlchemyResponse>(json);

                List<string> keywords = alchemyResponse.status.Equals("error", StringComparison.OrdinalIgnoreCase)
                           ? new List<string>()
                           : alchemyResponse.keywords
                                    .Where(k => k.relevance >= 0.5)
                                    .OrderByDescending(k => k.relevance)
                                    .Select(keyword => keyword.text)
                                    .ToList();

                return new ReadOnlyCollection<string>(keywords);
            }
            catch (Exception exception)
            {
                Logger.Error("[SEO] Error getting keywords from Alchemy", exception);
                return new ReadOnlyCollection<string>(new List<string>());
            }
        }

        /// <summary>
        /// Determines whether the specified self has attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo">The propertyInfo.</param>
        /// <returns><c>true</c> if the specified self has attribute; otherwise, <c>false</c>.</returns>
        private static bool HasAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            T attr = (T)Attribute.GetCustomAttribute(propertyInfo, typeof(T));

            return attr != null;
        }

        private void GetSettings()
        {
            string alchemyKey = ConfigurationManager.AppSettings["seo.alchemy.key"];

            PageData startPageData = this.ContentRepository.Service.Get<PageData>(ContentReference.StartPage);

            PropertyInfo keywordSettingsProperty =
                startPageData.GetType().GetProperties().Where(HasAttribute<KeywordGenerationSettingsAttribute>).FirstOrDefault();

            if (keywordSettingsProperty == null)
            {
                this.MaxItems = 20;
                this.AlchemyKey = alchemyKey;
                return;
            }

            KeywordGenerationSettingsBlock keywordGenerationSettings =
                startPageData[keywordSettingsProperty.Name] as KeywordGenerationSettingsBlock;

            if (keywordGenerationSettings == null)
            {
                this.MaxItems = 20;
                this.AlchemyKey = alchemyKey;
                return;
            }

            this.MaxItems = keywordGenerationSettings.MaxItems > 0 ? keywordGenerationSettings.MaxItems : 20;
            this.AlchemyKey = !string.IsNullOrWhiteSpace(keywordGenerationSettings.AlchemyKey) ? keywordGenerationSettings.AlchemyKey : alchemyKey;
        }
    }
}
