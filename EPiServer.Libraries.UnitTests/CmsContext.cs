// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CmsContext.cs" company="Joel Abrahamsson">
//   Copyright© 2013 Joel Abrahamsson. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;
    using System.Web.UI;

    using EPiServer.BaseLibrary;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.DataAnnotations;
    using EPiServer.Framework.Localization;
    using EPiServer.Security;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;
    using EPiServer.Web.Routing;

    using FakeItEasy;

    using Machine.Specifications.Annotations;

    /// <summary>
    ///     The cms context.
    /// </summary>
    public class CmsContext
    {
        #region Fields

        /// <summary>
        ///     The child pages.
        /// </summary>
        private readonly Dictionary<ContentReference, List<IContent>> childPages =
            new Dictionary<ContentReference, List<IContent>>();

        /// <summary>
        /// The page types.
        /// </summary>
        private readonly List<ContentType> pageTypes = new List<ContentType>();

        /// <summary>
        ///     The next page id.
        /// </summary>
        private int nextPageId = 5;

        /// <summary>
        /// The next page type id
        /// </summary>
        private int nextPageTypeId = 1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CmsContext" /> class.
        /// </summary>
        public CmsContext()
        {
            ContentReference.RootPage = new PageReference(1);
            
            ContentReference.StartPage = new PageReference(4);
            
            this.StartPageUrl = "/";

            Context.Current = A.Fake<IContext>();

            HttpContextBase = A.Fake<HttpContextBase>();

            ServiceLocator.SetLocator(A.Fake<IServiceLocator>());

            this.ContentLoader = A.Fake<IContentLoader>();

            this.UrlResolver = A.Fake<UrlResolver>();

            this.TemplateResolver = A.Fake<TemplateResolver>();

            this.ContentRepository = A.Fake<IContentRepository>();

            this.ContentTypeRepository = A.Fake<IContentTypeRepository>();

            this.LocalizationService = A.Fake<LocalizationService>();

            this.ProviderBasedLocalizationService = new ProviderBasedLocalizationService();

            A.CallTo(() => Context.Current.RequestTime).Returns(DateTime.Now);

            A.CallTo(() => ServiceLocator.Current.GetInstance<IContentRepository>()).Returns(this.ContentRepository);

            A.CallTo(() => ServiceLocator.Current.GetInstance<IContentTypeRepository>()).Returns(this.ContentTypeRepository);

            A.CallTo(() => ServiceLocator.Current.GetInstance<TemplateResolver>()).Returns(this.TemplateResolver);

            A.CallTo(() => this.UrlResolver.GetVirtualPath(ContentReference.StartPage)).Returns(this.StartPageUrl);

            A.CallTo(() => ServiceLocator.Current.GetInstance<ProviderBasedLocalizationService>()).Returns(this.ProviderBasedLocalizationService);

            A.CallTo(() => ServiceLocator.Current.GetInstance<LocalizationService>()).Returns(this.ProviderBasedLocalizationService);

            // Also fake the TryGetExistingInstance for the LocalizationService
            LocalizationService ignored;
            A.CallTo(() => ServiceLocator.Current.TryGetExistingInstance(out ignored)).Returns(true).AssignsOutAndRefParameters(this.ProviderBasedLocalizationService);

            this.RegisterMocks();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the http context.
        /// </summary>
        [NotNull]
        public static HttpContextBase HttpContextBase { get; set; }

        /// <summary>
        ///     Gets or sets the content loader.
        /// </summary>
        [NotNull]
        public IContentLoader ContentLoader { get; set; }

        /// <summary>
        ///     Gets or sets the start page url.
        /// </summary>
        [NotNull]
        public string StartPageUrl { get; set; }

        /// <summary>
        ///     Gets or sets the template resolver.
        /// </summary>
        [NotNull]
        public TemplateResolver TemplateResolver { get; set; }

        /// <summary>
        ///     Gets or sets the url resolver.
        /// </summary>
        [NotNull]
        public UrlResolver UrlResolver { get; set; }

        /// <summary>
        /// Gets or sets the content repository.
        /// </summary>
        [NotNull]
        public IContentRepository ContentRepository { get; set; }

        /// <summary>
        /// Gets or sets the content type repository.
        /// </summary>
        /// <value>
        /// The content type repository.
        /// </value>
        [NotNull]
        public IContentTypeRepository ContentTypeRepository { get; set; }

        /// <summary>
        /// Gets or sets the localization service.
        /// </summary>
        /// <value>
        /// The localization service.
        /// </value>
        [NotNull]
        public ProviderBasedLocalizationService ProviderBasedLocalizationService { get; set; }

        /// <summary>
        /// Gets or sets the localization service.
        /// </summary>
        /// <value>
        /// The localization service.
        /// </value>
        [NotNull]
        public LocalizationService LocalizationService { get; set; }

        /// <summary>
        /// Gets the master language.
        /// </summary>
        /// <value>
        /// The master language.
        /// </value>
        [NotNull]
        public CultureInfo MasterLanguage
        {
            get
            {
                return new CultureInfo("en-US");
            }
        }

        /// <summary>
        /// Gets the second language.
        /// </summary>
        /// <value>
        /// The second language.
        /// </value>
        [NotNull]
        public CultureInfo SecondLanguage
        {
            get
            {
                return new CultureInfo("nl");
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Creates the type of the page.
        /// </summary>
        /// <param name="pageType">Type of the page.</param>
        public void CreatePageType(Type pageType)
        {
            this.nextPageTypeId += 1;
            
            ContentTypeAttribute attribute =
                   (ContentTypeAttribute)
                   Attribute.GetCustomAttribute(pageType, typeof(ContentTypeAttribute));

            ContentType contentType = A.Fake<ContentType>();
            contentType.ID = this.nextPageTypeId;
            contentType.ModelType = pageType;
            contentType.Name = pageType.Name;
            contentType.GUID = attribute.GetGUID().GetValueOrDefault();
            contentType.Description = attribute.Description;
            contentType.DisplayName = attribute.DisplayName;
            contentType.IsAvailable = attribute.AvailableInEditMode;
            contentType.GroupName = "UnitTests";
            contentType.SortOrder = 100;
            contentType.ACL = new AccessControlList();

            this.pageTypes.Add(contentType);

            A.CallTo(() => this.ContentTypeRepository.Load(pageType.Name))
             .Returns(this.pageTypes.FirstOrDefault(pt => pt.Name.Equals(pageType.Name)));

            A.CallTo(() => this.ContentTypeRepository.Load(pageType)).Returns(this.pageTypes.FirstOrDefault(pt => pt.Name.Equals(pageType.Name)));
        }

        /// <summary>
        /// Creates a page and sets the properties.
        /// </summary>
        /// <typeparam name="T">The type of container to use.</typeparam>
        /// <param name="name">The name.</param>
        /// <param name="parentLink">The parent link.</param>
        /// <returns>
        /// The <see cref="ContentReference" />.
        /// </returns>
        [NotNull]
        public T CreateContent<T>([NotNull] string name, [NotNull] ContentReference parentLink) where T : PageData, new()
        {
            // Get the ContentType for the page you want to create
            ContentType contentType = this.ContentTypeRepository.Load(typeof(T));

            T page = new T();

            page.Property["PageWorkStatus"] = new PropertyNumber((int)VersionStatus.Published);
            page.Property["PageStartPublish"] = new PropertyDate(DateTime.Now.AddDays(-1));
            page.Property["PageName"] = new PropertyString(name);

            page.Property["MasterLanguageBranch"] = new PropertyString(CultureInfo.CurrentUICulture.Name);
            page.Property["PageLanguageBranch"] = new PropertyString(CultureInfo.CurrentUICulture.Name);
            page.Language = new CultureInfo(CultureInfo.CurrentUICulture.Name);

            page.Property["PageParentLink"] = new PropertyPageReference(parentLink);

            page.Property["PageLink"] = new PropertyPageReference();

            page.ContentLink = parentLink == ContentReference.RootPage
                                   ? ContentReference.StartPage
                                   : new PageReference(++this.nextPageId);

            page.ExistingLanguages = new List<CultureInfo> { new CultureInfo(CultureInfo.CurrentUICulture.Name) };

            // If no ContentType can be found, just create the ContentData without a ContentType
            if (contentType.ID > 0)
            {
                page.Property["PageTypeID"] = new PropertyNumber(contentType.ID);
                page.Property["PageTypeName"] = new PropertyString(contentType.Name);
                page.Property["ContentTypeID"] = new PropertyNumber(contentType.ID);
            }

            this.AddChild(page.ParentLink, page);

            return page;
        }

        /// <summary>
        /// Add a language version to a content item.
        /// </summary>
        /// <typeparam name="T">The type of container to use.</typeparam>
        /// <param name="contentLink">The content link.</param>
        /// <param name="languageSelector">The language selector.</param>
        /// <returns>
        /// The <see cref="ContentReference" />.
        /// </returns>
        /// <exception cref="EPiServer.Core.ContentNotFoundException">Content not found.</exception>
        /// <exception cref="EPiServer.Core.EPiServerException">Creating a copy of the item did not succeed.</exception>
        [NotNull]
        public T CreateLanguageVersionOfContent<T>([NotNull] ContentReference contentLink, [NotNull] LanguageSelector languageSelector) where T : PageData, new()
        {
            // Get the original page
            T page =
                this.ContentRepository.Get<ContentData>(
                    contentLink, new LanguageSelector(CultureInfo.CurrentUICulture.Name)) as T;

            if (page == null)
            {
                throw new ContentNotFoundException();
            }

            // Create a copy
            T languageVersion = page.CreateWritableClone() as T;

            if (languageVersion == null)
            {
                throw new EPiServerException("Creating a copy of the item did not succeed.");
            }

            // Set the language to the new version
            languageVersion.Language = new CultureInfo(languageSelector.LanguageBranch);

            // Set the contentlink to self, as it is a language version
            languageVersion.ContentLink = contentLink;

            // Add new language to the version
            languageVersion.ExistingLanguages = new List<CultureInfo>
                                                         {
                                                             new CultureInfo(page.LanguageBranch),
                                                             new CultureInfo(
                                                                 languageSelector.LanguageBranch)
                                                         };

            // Add new language to the original
            page.ExistingLanguages = new List<CultureInfo>
                                              {
                                                  new CultureInfo(page.LanguageBranch),
                                                  new CultureInfo(languageSelector.LanguageBranch)
                                              };

            this.AddChild(page.ParentLink, languageVersion);

            return languageVersion;
        }

        /// <summary>
        /// Gets the rendered control.
        /// </summary>
        /// <param name="control">The control to render.</param>
        /// <returns>The rendered control.</returns>
        [NotNull]
        public string GetRenderedControl([NotNull] Control control)
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());
            control.RenderControl(writer);
            return writer.InnerWriter.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add child.
        /// </summary>
        /// <param name="parentLink">
        /// The parent link.
        /// </param>
        /// <param name="child">
        /// The child.
        /// </param>
        private void AddChild(ContentReference parentLink, PageData child)
        {
            if (!this.childPages.ContainsKey(parentLink))
            {
                this.childPages.Add(parentLink, new List<IContent>());
            }
        
            this.childPages[parentLink].Add(child);

            // Add some specific mock calls for EPiServer calls, getting the pagedata from the container.
            A.CallTo(() => this.ContentRepository.Get<ContentData>(child.ContentLink)).Returns(this.childPages[parentLink].OfType<PageData>().First(p => p.ContentLink == child.ContentLink));
            
            A.CallTo(() => this.ContentRepository.Get<PageData>(child.ContentLink)).Returns(this.childPages[parentLink].OfType<PageData>().First(p => p.ContentLink == child.ContentLink));

            A.CallTo(() => this.ContentRepository.Get<ContentData>(child.ContentLink, A<LanguageSelector>.Ignored))
            .ReturnsLazily(m => this.childPages[parentLink].OfType<PageData>().First(p => p.ContentLink == child.ContentLink && p.LanguageBranch == m.GetArgument<LanguageSelector>(1).LanguageBranch));

            A.CallTo(() => this.ContentRepository.Get<PageData>(child.ContentLink, A<LanguageSelector>.Ignored))
           .ReturnsLazily(m => this.childPages[parentLink].OfType<PageData>().First(p => p.ContentLink == child.ContentLink && p.LanguageBranch == m.GetArgument<LanguageSelector>(1).LanguageBranch));

            A.CallTo(() => this.ContentRepository.GetLanguageBranches<PageData>(child.ContentLink)).Returns(this.childPages[parentLink].OfType<PageData>().Where(c => c.ContentLink == child.ContentLink));
        }

        #endregion

        /// <summary>
        /// Gets the "descendents".
        /// </summary>
        /// <param name="parentLink">The parent link.</param>
        /// <returns>A <see cref="List{ContentReference}"/></returns>
        [NotNull]
        private IEnumerable<ContentReference> GetAll([NotNull] ContentReference parentLink)
        {
            foreach (PageData child in this.childPages[parentLink])
            {
                yield return child.ContentLink;

                if (!this.childPages.ContainsKey(child.ContentLink))
                {
                    continue;
                }

                foreach (PageData grandChild in this.childPages[child.ContentLink])
                {
                    yield return grandChild.ContentLink;
                }
            }
        }

        /// <summary>
        /// Register some mocks for EPiServer calls for getting pagedata.
        /// </summary>
        private void RegisterMocks()
        {
            A.CallTo(() => this.ContentLoader.GetChildren<IContent>(A<ContentReference>.Ignored)).ReturnsLazily(m => this.childPages[m.GetArgument<ContentReference>(0)].OfType<PageData>());

            A.CallTo(() => this.ContentLoader.GetChildren<PageData>(A<ContentReference>.Ignored)).ReturnsLazily(m => this.childPages[m.GetArgument<ContentReference>(0)].OfType<PageData>());

            A.CallTo(() => this.ContentLoader.GetChildren<PageData>(A<ContentReference>.Ignored, A<LanguageSelector>.Ignored))
                .ReturnsLazily(m => this.childPages[m.GetArgument<ContentReference>(0)].OfType<PageData>().Where(p => p.LanguageBranch == m.GetArgument<LanguageSelector>(1).LanguageBranch));

            A.CallTo(() => this.ContentRepository.GetChildren<IContent>(A<ContentReference>.Ignored)).ReturnsLazily(m => this.childPages[m.GetArgument<ContentReference>(0)].OfType<PageData>());

            A.CallTo(() => this.ContentRepository.GetChildren<PageData>(A<ContentReference>.Ignored)).ReturnsLazily(m => this.childPages[m.GetArgument<ContentReference>(0)].OfType<PageData>());

            A.CallTo(() => this.ContentRepository.GetChildren<PageData>(A<ContentReference>.Ignored, A<LanguageSelector>.Ignored))
                .ReturnsLazily(m => this.childPages[m.GetArgument<ContentReference>(0)].OfType<PageData>().Where(p => p.LanguageBranch == m.GetArgument<LanguageSelector>(1).LanguageBranch));

            A.CallTo(() => this.ContentRepository.GetDescendents(A<ContentReference>.Ignored)).ReturnsLazily(m => this.GetAll(m.GetArgument<ContentReference>(0)).Distinct());
        }
    }
}