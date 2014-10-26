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
using System.Linq;
using System.Reflection;
using System.Text;

using EPi.Libraries.BlockSearch.DataAnnotations;

using EPiServer;
using EPiServer.Core;
using EPiServer.Core.Html;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;

namespace EPi.Libraries.BlockSearch
{
    /// <summary>
    /// Class SearchInitialization.
    /// </summary>
    [InitializableModule]
    [ModuleDependency(typeof(FrameworkInitialization))]
    public class SearchInitialization : IInitializableModule
    {
        #region Properties

        /// <summary>
        /// Gets or sets the content type respository.
        /// </summary>
        /// <value>The content type respository.</value>
        protected Injected<IContentTypeRepository> ContentTypeRepository { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks>Gets called as part of the EPiServer Framework initialization sequence. Note that it will be called
        /// only once per AppDomain, unless the method throws an exception. If an exception is thrown, the initialization
        /// method will be called repeadetly for each request reaching the site until the method succeeds.</remarks>
        public void Initialize(InitializationEngine context)
        {
            DataFactory.Instance.PublishingPage += this.OnPublishingPage;
            DataFactory.Instance.PublishedContent += this.OnPublishedContent;
        }

        /// <summary>
        /// Handles the <see cref="E:PublishedContent" /> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="pageEventArgs">The <see cref="ContentEventArgs"/> instance containing the event data.</param>
        public void OnPublishedContent(object sender, ContentEventArgs pageEventArgs)
        {
            if (pageEventArgs == null)
            {
                return;
            }

            // Check if the content that is published is indeed a block.
            BlockData blockData = pageEventArgs.Content as BlockData;

            // If it's not, don't do anything.
            if (blockData == null)
            {
                return;
            }

            // Get the softlink repository.
            ContentSoftLinkRepository linkRepository = ServiceLocator.Current.GetInstance<ContentSoftLinkRepository>();

            // Get the references to this block
            List<ContentReference> referencingContentLinks =
                linkRepository.Load(pageEventArgs.ContentLink, true)
                    .Where(
                        link =>
                        link.SoftLinkType == ReferenceType.PageLinkReference
                        && !ContentReference.IsNullOrEmpty(link.OwnerContentLink))
                    .Select(link => link.OwnerContentLink)
                    .ToList();

            // Loop through each reference
            foreach (ContentReference referencingContentLink in referencingContentLinks)
            {
                PageData parent;
                DataFactory.Instance.TryGet(referencingContentLink, out parent);

                // If it is not a standard page, do nothing
                if (parent == null)
                {
                    continue;
                }

                // Check if the containing page is published.
                if (!parent.IsPendingPublish)
                {
                    // Republish the containing page.
                    DataFactory.Instance.Save(parent.CreateWritableClone(), SaveAction.Publish);
                }
            }
        }

        /// <summary>
        /// Raises the page event.
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

            PropertyInfo addtionalSearchContentProperty = GetAddtionalSearchContentProperty(page);

            if (addtionalSearchContentProperty == null)
            {
                return;
            }

            if (addtionalSearchContentProperty.PropertyType != typeof(string))
            {
                return;
            }

            StringBuilder stringBuilder = new StringBuilder();

            ContentType contentType = this.ContentTypeRepository.Service.Load(page.ContentTypeID);

            foreach (PropertyDefinition current in
                from d in contentType.PropertyDefinitions
                where typeof(PropertyContentArea).IsAssignableFrom(d.Type.DefinitionType)
                select d)

            {
                PropertyData propertyData = page.Property[current.Name];

                ContentArea contentArea = propertyData.Value as ContentArea;

                if (contentArea == null)
                {
                    continue;
                }

                foreach (ContentAreaItem contentAreaItem in contentArea.Items)
                {
                    IContent blockData = contentAreaItem.GetContent();

                    IEnumerable<string> props = this.GetSearchablePropertyValues(blockData, blockData.ContentTypeID);

                    stringBuilder.AppendFormat(" {0}", string.Join(" ", props));
                }
            }

            if (addtionalSearchContentProperty.PropertyType == typeof(string))
            {
                page[addtionalSearchContentProperty.Name] = TextIndexer.StripHtml(stringBuilder.ToString(), 0);
            }
        }

        /// <summary>
        /// Preloads the module.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <remarks>This method is only available to be compatible with "AlwaysRunning" applications in .NET 4 / IIS 7.
        /// It currently serves no purpose.</remarks>
        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        /// Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <remarks><para>
        /// This method is usually not called when running under a web application since the web app may be shut down very
        /// abruptly, but your module should still implement it properly since it will make integration and unit testing
        /// much simpler.
        /// </para>
        /// <para>
        /// Any work done by <see cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)" /> as well as any code executing on <see cref="E:EPiServer.Framework.Initialization.InitializationEngine.InitComplete" /> should be reversed.
        /// </para></remarks>
        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.PublishingPage -= this.OnPublishingPage;
            DataFactory.Instance.PublishedContent -= this.OnPublishedContent;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the name of the key word property.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>System.String.</returns>
        private static PropertyInfo GetAddtionalSearchContentProperty(PageData page)
        {
            PropertyInfo keywordsMetatagProperty =
                page.GetType().GetProperties().Where(HasAttribute<AdditionalSearchContentAttribute>).FirstOrDefault();

            return keywordsMetatagProperty;
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

        /// <summary>
        /// Gets the searchable property values.
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
        /// Gets the searchable property values.
        /// </summary>
        /// <param name="contentData">The content data.</param>
        /// <param name="contentTypeID">The content type identifier.</param>
        /// <returns>IEnumerable&lt;System.String&gt;.</returns>
        private IEnumerable<string> GetSearchablePropertyValues(IContentData contentData, int contentTypeID)
        {
            return this.GetSearchablePropertyValues(
                contentData,
                this.ContentTypeRepository.Service.Load(contentTypeID));
        }

        #endregion
    }
}