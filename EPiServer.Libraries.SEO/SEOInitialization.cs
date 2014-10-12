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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;

using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.Libraries.SEO.DataAnnotations;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;

using log4net;

namespace EPiServer.Libraries.SEO
{
    /// <summary>
    ///     Class SEOInitialization.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class SEOInitialization : IInitializableModule
    {
        #region Static Fields

        /// <summary>
        ///     The logger
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SEOInitialization));

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the content repository.
        /// </summary>
        /// <value>The content repository.</value>
        protected Injected<IContentRepository> ContentRepository { get; set; }

        /// <summary>
        ///     Gets or sets the content type respository.
        /// </summary>
        /// <value>The content type respository.</value>
        protected Injected<IContentTypeRepository> ContentTypeRepository { get; set; }

        /// <summary>
        /// Gets or sets the extraction service.
        /// </summary>
        /// <value>The extraction service.</value>
        protected Injected<IExtractionService> ExtractionService { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        ///     Gets called as part of the EPiServer Framework initialization sequence. Note that it will be called
        ///     only once per AppDomain, unless the method throws an exception. If an exception is thrown, the initialization
        ///     method will be called repeadetly for each request reaching the site until the method succeeds.
        /// </remarks>
        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.PublishingPage += this.OnPublishingPage;
        }

        /// <summary>
        ///     Raises the page event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="pageEventArgs">Event information to send to registered event handlers.</param>
        public void OnPublishingPage(object sender, PageEventArgs pageEventArgs)
        {
            if (pageEventArgs == null)
            {
                return;
            }

            PageData page = pageEventArgs.Page;

            if (page == null)
            {
                return;
            }

            PropertyInfo keywordsMetatagProperty = GetKeyWordProperty(page);

            if (keywordsMetatagProperty == null)
            {
                return;
            }

            IEnumerable<string> props = GetSearchablePropertyValues(page, page.ContentTypeID);

            string textToAnalyze = TextIndexer.StripHtml(string.Join(" ", props), 0);

            ReadOnlyCollection<string> keywordList;

            try
            {
                keywordList = this.ExtractionService.Service.GetKeywords(textToAnalyze);
            }
            catch (ActivationException activationException)
            {
                Logger.Error("[SEO] No extraction service available", activationException);
                return;
            }

            if (keywordList.Count == 0)
            {
                return;
            }

            if (keywordsMetatagProperty.PropertyType == typeof(string[]))
            {
                page[keywordsMetatagProperty.Name] = keywordList.ToArray();
            }
            else if (keywordsMetatagProperty.PropertyType == typeof(List<string>))
            {
                page[keywordsMetatagProperty.Name] = keywordList;
            }
            else if (keywordsMetatagProperty.PropertyType == typeof(string))
            {
                page[keywordsMetatagProperty.Name] = string.Join(",", keywordList);
            }
        }

        /// <summary>
        ///     Preloads the module.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>
        ///     This method is only available to be compatible with "AlwaysRunning" applications in .NET 4 / IIS 7.
        ///     It currently serves no purpose.
        /// </remarks>
        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        ///     Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>
        ///     <para>
        ///         This method is usually not called when running under a web application since the web app may be shut down very
        ///         abruptly, but your module should still implement it properly since it will make integration and unit testing
        ///         much simpler.
        ///     </para>
        ///     <para>
        ///         Any work done by
        ///         <see
        ///             cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)" />
        ///         as well as any code executing on
        ///         <see cref="E:EPiServer.Framework.Initialization.InitializationEngine.InitComplete" /> should be reversed.
        ///     </para>
        /// </remarks>
        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.PublishingPage -= this.OnPublishingPage;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Gets the name of the key word property.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>PropertyInfo.</returns>
        private static PropertyInfo GetKeyWordProperty(PageData page)
        {
            PropertyInfo keywordsMetatagProperty =
                page.GetType().GetProperties().Where(HasAttribute<KeywordsMetaTagAttribute>).FirstOrDefault();

            return keywordsMetatagProperty;
        }

        /// <summary>
        ///     Determines whether the specified self has attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyInfo">The propertyInfo.</param>
        /// <returns><c>true</c> if the specified self has attribute; otherwise, <c>false</c>.</returns>
        private static bool HasAttribute<T>(PropertyInfo propertyInfo) where T : Attribute
        {
            T attr = (T)Attribute.GetCustomAttribute(propertyInfo, typeof(T));

            return attr != null;
        }

        /// <summary>
        ///     Gets the searchable property values.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        private IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, ContentType contentType)
        {
            if (contentType == null)
            {
                yield break;
            }

            foreach (PropertyDefinition current in
                from d in contentType.PropertyDefinitions
                where d.Searchable || typeof(IPropertyBlock).IsAssignableFrom(d.Type.DefinitionType)
                select d)
            {
                PropertyData propertyData = contentData.Property[current.Name];
                IPropertyBlock propertyBlock = propertyData as IPropertyBlock;
                if (propertyBlock != null)
                {
                    foreach (string current2 in
                        this.GetSearchablePropertyValues(
                            propertyBlock.Block,
                            propertyBlock.BlockPropertyDefinitionTypeID))
                    {
                        yield return current2;
                    }
                }
                else
                {
                    yield return propertyData.ToWebString();
                }
            }
        }

        /// <summary>
        ///     Gets the searchable property values.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <param name="contentTypeID">The content type identifier.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        private IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, int contentTypeID)
        {
            return this.GetSearchablePropertyValues(contentData, this.ContentTypeRepository.Service.Load(contentTypeID));
        }

        #endregion
    }
}