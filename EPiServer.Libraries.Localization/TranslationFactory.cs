// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TranslationFactory.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Localization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Linq;

    using EPiServer.Core;
    using EPiServer.Libraries.Localization.Models;
    using EPiServer.ServiceLocation;

    using log4net;

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
                if (instance == null)
                {
                    lock (SyncLock)
                    {
                        if (instance == null)
                        {
                            instance = new TranslationFactory();
                        }
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
            MemoryStream ms = null;

            try
            {
                ms = new MemoryStream();

                XmlWriter xw = XmlWriter.Create(
                    ms, new XmlWriterSettings { Indent = true, OmitXmlDeclaration = false, Encoding = Encoding.UTF8 });

                xw.WriteStartDocument();
                xw.WriteStartElement("languages");

                foreach (CultureInfo cultureInfo in this.AvailableLanguages)
                {
                    xw.WriteStartElement("language");
                    xw.WriteAttributeString("name", cultureInfo.NativeName);
                    xw.WriteAttributeString("id", cultureInfo.Name);

                    this.AddElement(xw, this.TranslationContainerReference, cultureInfo);

                    xw.WriteEndElement();
                }

                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Flush();

                ms.Position = 0;

                XmlReader xr = XmlReader.Create(ms);

                returnXml = XElement.Load(xr);

                xw.Close();
            }
            catch (Exception)
            {
                if (ms != null)
                {
                    ms.Dispose();
                }
            }

            return returnXml != null ? returnXml.ToString() : XDocument.Parse(BackupXML).ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Add a category element.
        /// </summary>
        /// <param name="xmlWriter">
        /// The xmlWriter.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
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
        /// The add element.
        /// </summary>
        /// <param name="xw">
        /// The xmlWriter.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
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
                        translationContainer.Name.ToLowerInvariant(), @"[^A-Za-z0-9]+", string.Empty);
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
                        translationItem.OriginalText.ToLowerInvariant(), @"[^A-Za-z0-9]+", string.Empty);
                    xw.WriteElementString(key, translationItem.Translation);
                }
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
            PageReference containerPageReference = ContentReference.StartPage;

            this.ContentRepository.Get<ContentData>(ContentReference.StartPage)
                    .GetPropertyValue("TranslationContainer", ContentReference.StartPage);

            if (containerPageReference == ContentReference.StartPage)
            {
                Logger.Info("[Localization] No translation container specified.");

                TranslationContainer containerReference =
                    this.ContentRepository.GetChildren<PageData>(containerPageReference).OfType<TranslationContainer>().FirstOrDefault();

                if (containerReference != null)
                {
                    Logger.Info("[Localization] First translation container used.");

                    containerPageReference = containerReference.PageLink;
                }
            }

            return containerPageReference;
        }

        #endregion
    }
}