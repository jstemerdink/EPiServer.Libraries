// Copyright© 2014 Jeroen Stemerdink. All Rights Reserved.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Xml;

using EPi.Libraries.Localization.Bing.Models;

using EPiServer.ServiceLocation;

using log4net;

namespace EPi.Libraries.Localization.Bing
{
    /// <summary>
    /// Class TranslationService.
    /// </summary>
     [ServiceConfiguration(typeof(ITranslationService))]
    public class TranslationService : ITranslationService
    {
        #region Constants

        /// <summary>
        ///     The url to Bing authentication
        /// </summary>
        private const string TranslatorAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";

        /// <summary>
        ///     The url to Bing translation
        /// </summary>
        private const string TranslatorUri =
            "http://api.microsofttranslator.com/v2/Http.svc/Translate?text={0}&from={1}&to={2}";

        #endregion

        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="TranslationService" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TranslationService));

        #endregion

        #region Fields

        /// <summary>
        ///     The bing access token
        /// </summary>
        private BingAccessToken bingAccessToken;

        /// <summary>
        ///     The bing client identifier
        /// </summary>
        private string bingClientID;

        /// <summary>
        ///     The bing client secret
        /// </summary>
        private string bingClientSecret;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the Bing access token.
        /// </summary>
        /// <value>The adm access token.</value>
        public BingAccessToken BingAccessToken
        {
            get
            {
                return this.bingAccessToken ?? (this.bingAccessToken = this.GetAccesToken());
            }
        }

        /// <summary>
        ///     Gets the Bing client identifier.
        /// </summary>
        /// <value>The Bing client identifier.</value>
        public string BingClientID
        {
            get
            {
                return this.bingClientID
                       ?? (this.bingClientID = ConfigurationManager.AppSettings["localization.bing.clientid"]);
            }
        }

        /// <summary>
        ///     Gets the Bing client identifier.
        /// </summary>
        /// <value>The Bing client identifier.</value>
        public string BingClientSecret
        {
            get
            {
                return this.bingClientSecret
                       ?? (this.bingClientSecret = ConfigurationManager.AppSettings["localization.bing.clientsecret"]);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Translates the specified text.
        /// </summary>
        /// <param name="toBeTranslated">The text to translate.</param>
        /// <param name="fromLang">From language.</param>
        /// <param name="toLang">To language.</param>
        /// <returns>System.String.</returns>
        public string Translate(string toBeTranslated, string fromLang, string toLang)
        {
            string headerValue = string.Format(
                CultureInfo.InvariantCulture,
                "Bearer {0}",
                this.BingAccessToken.access_token);

            Uri uri;

            try
            {
                uri = new Uri(string.Format(
                    CultureInfo.InvariantCulture,
                    TranslatorUri,
                    HttpUtility.UrlEncode(toBeTranslated),
                    fromLang,
                    toLang));
            }
            catch (UriFormatException uriFormatException)
            {
                Logger.Error("[Localization] URI for Bing in wrong format.", uriFormatException);
                return null;
            }

            try
            {
                WebRequest translationWebRequest = WebRequest.Create(uri);
                translationWebRequest.Headers.Add("Authorization", headerValue);

                WebResponse response = translationWebRequest.GetResponse();
                Stream stream = response.GetResponseStream();
                Encoding encode = Encoding.GetEncoding("utf-8");

                if (stream == null)
                {
                    return null;
                }

                StreamReader translatedStream = new StreamReader(stream, encode);
                XmlDocument xTranslation = new XmlDocument();
                xTranslation.LoadXml(translatedStream.ReadToEnd());

                return xTranslation.InnerText;
            }
            catch (Exception exception)
            {
                Logger.Error("[Localization] Error getting translations from Bing", exception);
                return null;
            }
        }

        #endregion

        #region Methods

        private BingAccessToken GetAccesToken()
        {
            if (string.IsNullOrWhiteSpace(this.BingClientID) | string.IsNullOrWhiteSpace(this.BingClientSecret))
            {
                return null;
            }

            string requestDetails = string.Format(
                CultureInfo.InvariantCulture,
                "grant_type=client_credentials&client_id={0}&client_secret={1} &scope=http://api.microsofttranslator.com",
                HttpUtility.UrlEncode(this.BingClientID),
                HttpUtility.UrlEncode(this.BingClientSecret));

            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);

            try
            {
                WebRequest webRequest = WebRequest.Create(TranslatorAccessUri);
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.Method = "POST";
                webRequest.ContentLength = bytes.Length;

                using (Stream outputStream = webRequest.GetRequestStream())
                {
                    outputStream.Write(bytes, 0, bytes.Length);
                }

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(BingAccessToken));

                WebResponse webResponse = webRequest.GetResponse();

                Stream responseStream = webResponse.GetResponseStream();

                if (responseStream == null)
                {
                    return null;
                }

                BingAccessToken token = (BingAccessToken)serializer.ReadObject(responseStream);

                return token;
            }
            catch (Exception exception)
            {
                Logger.Error("[Localization] Error getting authentication from Bing", exception);
                return null;
            }
        }

        #endregion
    }
}