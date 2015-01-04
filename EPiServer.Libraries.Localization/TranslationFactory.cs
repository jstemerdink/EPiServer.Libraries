// Copyright© 2015 Jeroen Stemerdink. All Rights Reserved.
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
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;

using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Libraries.Localization.Models;
using EPiServer.Security;
using EPiServer.ServiceLocation;

using log4net;

namespace EPiServer.Libraries.Localization
{
    /// <summary>
    ///     The TranslationFactory class, used for translation queries.
    /// </summary>
    public sealed class TranslationFactory
    {
        #region Constants

        /// <summary>
        ///     The backup xml, to be returned when something goes wrong.
        /// </summary>
        private const string BackupXML = @"<?xml version='1.0' encoding='utf-8'?><languages></languages>";

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
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="TranslationFactory" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(TranslationFactory));

        /// <summary>
        ///     The synclock object.
        /// </summary>
        private static readonly object SyncLock = new object();

        /// <summary>
        ///     The one and only TranslationFactory instance.
        /// </summary>
        private static volatile TranslationFactory instance;

        #endregion

        #region Fields

        /// <summary>
        ///     The available languages
        /// </summary>
        private IEnumerable<CultureInfo> availableLanguages;

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

        /// <summary>
        ///     The content repository
        /// </summary>
        private IContentRepository contentRepository;

        /// <summary>
        ///     The translation container reference
        /// </summary>
        private PageReference translationContainerReference;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="TranslationFactory" /> class from being created.
        /// </summary>
        private TranslationFactory()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance of the TranslationFactory object.
        /// </summary>
        public static TranslationFactory Instance
        {
            get
            {
                // Double checked locking
                if (instance != null)
                {
                    return instance;
                }

                lock (SyncLock)
                {
                    if (instance == null)
                    {
                        instance = new TranslationFactory();
                    }
                }

                return instance;
            }
        }

        /// <summary>
        ///     Gets the available languages.
        /// </summary>
        public IEnumerable<CultureInfo> AvailableLanguages
        {
            get
            {
                return this.availableLanguages ?? (this.availableLanguages = this.GetAvailableLanguages());
            }
        }

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

        /// <summary>
        ///     Gets the reference to the translation container.
        /// </summary>
        public PageReference TranslationContainerReference
        {
            get
            {
                return this.translationContainerReference
                       ?? (this.translationContainerReference = this.GetTranslationContainer());
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the content repository.
        /// </summary>
        private IContentRepository ContentRepository
        {
            get
            {
                return this.contentRepository
                       ?? (this.contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Generate the language xml from the translation containers and items.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetXDocument()
        {
            XElement returnXml = null;
            MemoryStream memoryStream = null;

            try
            {
                memoryStream = new MemoryStream();

                XmlWriter xmlWriter = XmlWriter.Create(
                    memoryStream,
                    new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false, Encoding = Encoding.UTF8 });

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("languages");

                foreach (CultureInfo cultureInfo in this.AvailableLanguages)
                {
                    xmlWriter.WriteStartElement("language");
                    xmlWriter.WriteAttributeString("name", cultureInfo.NativeName);
                    xmlWriter.WriteAttributeString("id", cultureInfo.Name);

                    this.AddElement(xmlWriter, this.TranslationContainerReference, cultureInfo);

                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndDocument();
                xmlWriter.Flush();

                memoryStream.Position = 0;

                XmlReader xr = XmlReader.Create(memoryStream);

                returnXml = XElement.Load(xr);

                xmlWriter.Close();
            }
            catch (Exception)
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                }
            }

            return returnXml != null ? returnXml.ToString() : XDocument.Parse(BackupXML).ToString();
        }

        /// <summary>
        ///     Translates them all.
        /// </summary>
        /// <param name="page">The page.</param>
        public void TranslateThemAll(PageData page)
        {
            if (this.BingAccessToken == null)
            {
                return;
            }

            ILanguageBranchRepository languageBrancheRepository =
                ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();

            List<LanguageBranch> enabledLanguages = languageBrancheRepository.ListEnabled().ToList();

            foreach (LanguageBranch languageBranch in
                enabledLanguages.Where(lb => lb.Culture.Name != page.LanguageBranch))
            {
                this.CreateLanguageBranch(page, languageBranch.Culture.Name);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Add a category element.
        /// </summary>
        /// <param name="xmlWriter">
        ///     The xmlWriter.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="cultureInfo">
        ///     The culture info.
        /// </param>
        private void AddCategoryElement(XmlWriter xmlWriter, ContentReference container, CultureInfo cultureInfo)
        {
            List<ContentReference> children = this.ContentRepository.GetDescendents(container).ToList();

            foreach (TranslationItem translationItem in
                children.Select(
                    contentReference =>
                    this.ContentRepository.Get<PageData>(contentReference, new LanguageSelector(cultureInfo.Name)))
                    .OfType<TranslationItem>()
                    .Select(page => page))
            {
                xmlWriter.WriteStartElement("category");
                xmlWriter.WriteAttributeString("name", translationItem.OriginalText);
                xmlWriter.WriteElementString("description", translationItem.Translation);
                xmlWriter.WriteEndElement();
            }
        }

        /// <summary>
        ///     The add element.
        /// </summary>
        /// <param name="xw">
        ///     The xmlWriter.
        /// </param>
        /// <param name="container">
        ///     The container.
        /// </param>
        /// <param name="cultureInfo">
        ///     The culture info.
        /// </param>
        private void AddElement(XmlWriter xw, ContentReference container, CultureInfo cultureInfo)
        {
            List<PageData> children =
                this.ContentRepository.GetChildren<PageData>(container, new LanguageSelector(cultureInfo.Name)).ToList();

            foreach (PageData child in children)
            {
                TranslationContainer translationContainer = child as TranslationContainer;

                if (translationContainer != null)
                {
                    string key = Regex.Replace(
                        translationContainer.ContainerName.ToLowerInvariant(),
                        @"[^A-Za-z0-9]+",
                        string.Empty);
                    xw.WriteStartElement(key);

                    this.AddElement(xw, child.PageLink, cultureInfo);

                    xw.WriteEndElement();
                }

                CategoryTranslationContainer categoryTranslationContainer = child as CategoryTranslationContainer;

                if (categoryTranslationContainer != null)
                {
                    xw.WriteStartElement("categories");

                    this.AddCategoryElement(xw, child.PageLink, cultureInfo);

                    xw.WriteEndElement();
                }

                TranslationItem translationItem = child as TranslationItem;

                if (translationItem != null)
                {
                    string key = Regex.Replace(
                        translationItem.OriginalText.ToLowerInvariant(),
                        @"[^A-Za-z0-9]+",
                        string.Empty);
                    xw.WriteElementString(key, translationItem.Translation);
                }
            }
        }

        private string BingTranslate(string tobetranslated, string fromLang, string toLang)
        {
            string headerValue = string.Format(
                CultureInfo.InvariantCulture,
                "Bearer {0}",
                this.BingAccessToken.access_token);

            string uri = string.Format(
                CultureInfo.InvariantCulture,
                TranslatorUri,
                HttpUtility.UrlEncode(tobetranslated),
                fromLang,
                toLang);

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

        private void CreateLanguageBranch(PageData page, string languageBranch)
        {
            // Check if language already exists
            bool languageExists =
                this.ContentRepository.GetLanguageBranches<PageData>(page.PageLink)
                    .Any(p => string.Compare(p.LanguageBranch, languageBranch, StringComparison.OrdinalIgnoreCase) == 0);

            if (languageExists)
            {
                return;
            }

            TranslationItem translationItem = page as TranslationItem;

            if (translationItem != null)
            {
                TranslationItem languageItemVersion =
                    this.ContentRepository.CreateLanguageBranch<TranslationItem>(
                        page.PageLink,
                        new LanguageSelector(languageBranch));

                languageItemVersion.PageName = page.PageName;
                languageItemVersion.URLSegment = page.URLSegment;

                string translatedText = this.BingTranslate(
                    translationItem.OriginalText,
                    page.LanguageID.Split(new char['-'])[0],
                    languageItemVersion.LanguageID.Split(new char['-'])[0]);

                if (translatedText == null)
                {
                    return;
                }

                languageItemVersion.Translation = translatedText;

                if (!string.IsNullOrWhiteSpace(languageItemVersion.Translation))
                {
                    this.ContentRepository.Save(languageItemVersion, SaveAction.Publish, AccessLevel.NoAccess);
                }
            }
            else
            {
                PageData languageVersion = this.ContentRepository.CreateLanguageBranch<PageData>(
                    page.PageLink,
                    new LanguageSelector(languageBranch));

                languageVersion.PageName = page.PageName;
                languageVersion.URLSegment = page.URLSegment;

                this.ContentRepository.Save(languageVersion, SaveAction.Publish, AccessLevel.NoAccess);
            }
        }

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

        /// <summary>
        ///     Gets the available languages.
        /// </summary>
        /// <returns>
        ///     An <see cref="IEnumerable{CultureInfo}" />
        /// </returns>
        private IEnumerable<CultureInfo> GetAvailableLanguages()
        {
            if (PageReference.IsNullOrEmpty(this.TranslationContainerReference))
            {
                return new List<CultureInfo>();
            }

            IEnumerable<CultureInfo> languages =
                this.ContentRepository.GetLanguageBranches<PageData>(this.TranslationContainerReference)
                    .Select(pageData => pageData.Language)
                    .ToList();

            return languages;
        }

        /// <summary>
        ///     Get the translation container.
        /// </summary>
        /// <returns>
        ///     The <see cref="PageReference" /> to the translation container.
        /// </returns>
        private PageReference GetTranslationContainer()
        {
            PageReference containerPageReference;

            TranslationContainer containerReference =
                this.ContentRepository.GetChildren<PageData>(ContentReference.RootPage)
                    .OfType<TranslationContainer>()
                    .FirstOrDefault();

            if (containerReference != null)
            {
                Logger.Info("[Localization] First translation container under RootPage used.");
                containerPageReference = containerReference.PageLink;
                return containerPageReference;
            }

            if (PageReference.IsNullOrEmpty(ContentReference.StartPage))
            {
                return PageReference.EmptyReference;
            }

            containerPageReference =
                this.ContentRepository.Get<ContentData>(ContentReference.StartPage)
                    .GetPropertyValue("TranslationContainer", ContentReference.StartPage);

            if (containerPageReference != ContentReference.StartPage)
            {
                return containerPageReference;
            }

            Logger.Info("[Localization] No translation container specified.");

            containerReference =
                this.ContentRepository.GetChildren<PageData>(containerPageReference)
                    .OfType<TranslationContainer>()
                    .FirstOrDefault();

            if (containerReference == null)
            {
                return containerPageReference;
            }

            Logger.Info("[Localization] First translation container used.");

            containerPageReference = containerReference.PageLink;

            return containerPageReference;
        }

        #endregion
    }
}