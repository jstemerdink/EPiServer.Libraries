namespace EPiServer.Libraries.ContentProviders.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;

    using EPiServer.Configuration;
    using EPiServer.Core;
    using EPiServer.DataAbstraction;
    using EPiServer.Filters;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;

    using log4net;

    /// <summary>
    ///     Used to clone a part of the page tree
    /// </summary>
    /// <remarks>
    ///     The current implementation only supports cloning of <see cref="PageData" /> content
    /// </remarks>
    /// <code>
    ///  <![CDATA[ 
    ///  var rootPageOfContentToClone = new PageReference(10);
    ///  
    ///  var pageWhereClonedContentShouldAppear = new PageReference(20);
    ///  
    ///  var provider = new ClonedContentProvider(rootPageOfContentToClone, pageWhereClonedContentShouldAppear);
    /// 
    ///  var providerManager = ServiceLocator.Current.GetInstance<IContentProviderManager>();
    ///  
    ///  providerManager.ProviderMap.AddProvider(provider);
    ///  ]]>
    /// </code>
    public sealed class ClonedContentProvider : ContentProvider, IPageCriteriaQueryService
    {
        #region Static Fields

        /// <summary>
        ///     The logger.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ClonedContentProvider));

        #endregion

        #region Fields

        /// <summary>
        ///     The parameters.
        /// </summary>
        private readonly NameValueCollection parameters = new NameValueCollection(1);

        /// <summary>
        ///     The provider key.
        /// </summary>
        private string providerKey;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ClonedContentProvider"/> class.
        /// </summary>
        /// <param name="cloneRoot">
        /// The clone root.
        /// </param>
        /// <param name="entryRoot">
        /// The entry root.
        /// </param>
        public ClonedContentProvider(PageReference cloneRoot, PageReference entryRoot)
            : this(null, cloneRoot, entryRoot, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClonedContentProvider"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="cloneRoot">
        /// The clone root.
        /// </param>
        /// <param name="entryRoot">
        /// The entry root.
        /// </param>
        public ClonedContentProvider(string providerKey, PageReference cloneRoot, PageReference entryRoot)
            : this(providerKey, cloneRoot, entryRoot, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClonedContentProvider"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <param name="cloneRoot">
        /// The clone root.
        /// </param>
        /// <param name="entryRoot">
        /// The entry root.
        /// </param>
        /// <param name="categoryFilter">
        /// The category filter.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Parameter "cloneRoot" cannot be null or empty
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Entry root and clone root cannot be set to the same content reference
        ///     or
        ///     Unable to create ClonedContentProvider, the EntryRoot property must point to leaf content (without children)
        /// </exception>
        public ClonedContentProvider(
            string providerKey, PageReference cloneRoot, PageReference entryRoot, CategoryList categoryFilter)
        {
            if (PageReference.IsNullOrEmpty(cloneRoot))
            {
                throw new ArgumentNullException("cloneRoot");
            }

            if (cloneRoot.CompareToIgnoreWorkID(entryRoot))
            {
                throw new NotSupportedException("Entry root and clone root cannot be set to the same content reference");
            }

            if (DataFactory.Instance.GetChildren<IContent>(entryRoot).Any())
            {
                throw new NotSupportedException(
                    "Unable to create ClonedContentProvider, the EntryRoot property must point to leaf content (without children)");
            }

            this.CloneRoot = cloneRoot;
            this.EntryRoot = entryRoot;
            this.Category = categoryFilter;
            this.providerKey = providerKey;

            // Set the entry point parameter
            this.Parameters.Add(PageProvider.EntryPointString, this.EntryRoot.ID.ToString(CultureInfo.InvariantCulture));

            // Configure content store used to retrieve pages
            this.ContentStore = ServiceLocator.Current.GetInstance<ContentStore>();
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets capabilities indicating no content editing can be performed through this provider
        /// </summary>
        public override PageProviderCapabilities Capabilities
        {
            get
            {
                return PageProviderCapabilities.Search | PageProviderCapabilities.MultiLanguage;
            }
        }

        /// <summary>
        ///     Gets the category filters used for this content provider.
        /// </summary>
        /// <remarks>If set, pages not matching at least one of these categories will be excluded from this content provider</remarks>
        public CategoryList Category { get; private set; }

        /// <summary>
        ///     Gets the root of the content that should be cloned.
        /// </summary>
        public PageReference CloneRoot { get; private set; }

        /// <summary>
        ///     Gets the page where the cloned content will appear.
        /// </summary>
        public PageReference EntryRoot { get; private set; }

        /// <summary>
        ///     Gets configuration parameters for this content provider instance
        /// </summary>
        public override NameValueCollection Parameters
        {
            get
            {
                return this.parameters;
            }
        }

        /// <summary>
        ///     Gets a unique key for this content provider instance
        /// </summary>
        public override string ProviderKey
        {
            get
            {
                return string.IsNullOrWhiteSpace(this.providerKey)
                           ? string.Format(
                               CultureInfo.InvariantCulture, 
                               "ClonedContent-{0}-{1}", 
                               this.CloneRoot.ID, 
                               this.EntryRoot.ID)
                           : this.providerKey;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the content store used to get original content
        /// </summary>
        private ContentStore ContentStore { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The find all pages with criteria.
        /// </summary>
        /// <param name="pageLink">
        /// The page link.
        /// </param>
        /// <param name="criterias">
        /// The criterias.
        /// </param>
        /// <param name="languageBranch">
        /// The language branch.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The <see cref="PageDataCollection"/>.
        /// </returns>
        public PageDataCollection FindAllPagesWithCriteria(
            PageReference pageLink, 
            PropertyCriteriaCollection criterias, 
            string languageBranch, 
            ILanguageSelector selector)
        {
            if (PageReference.IsNullOrEmpty(pageLink))
            {
                return new PageDataCollection();
            }

            // Any search beneath the entry root should in fact be performed under the clone root as that's where the original content resides
            if (pageLink.CompareToIgnoreWorkID(this.EntryRoot))
            {
                pageLink = this.CloneRoot;
            }
            else if (!string.IsNullOrWhiteSpace(pageLink.ProviderName))
            {
                // Any search beneath a cloned page should in fact be performed under the original page, so we use a page link without any provider information
                pageLink = new PageReference(pageLink.ID);
            }

            PageDataCollection pages = DataFactory.Instance.FindAllPagesWithCriteria(
                pageLink, criterias, languageBranch, selector);

            // Return cloned search result set
            return new PageDataCollection(pages.Select(this.ClonePage));
        }

        /// <summary>
        /// The find pages with criteria.
        /// </summary>
        /// <param name="pageLink">
        /// The page link.
        /// </param>
        /// <param name="criterias">
        /// The criterias.
        /// </param>
        /// <param name="languageBranch">
        /// The language branch.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// The <see cref="PageDataCollection"/>.
        /// </returns>
        public PageDataCollection FindPagesWithCriteria(
            PageReference pageLink, 
            PropertyCriteriaCollection criterias, 
            string languageBranch, 
            ILanguageSelector selector)
        {
            if (PageReference.IsNullOrEmpty(pageLink))
            {
                return new PageDataCollection();
            }

            // Any search beneath the entry root should in fact be performed under the clone root as that's where the original content resides
            if (pageLink.CompareToIgnoreWorkID(this.EntryRoot))
            {
                pageLink = this.CloneRoot;
            }
            else if (!string.IsNullOrWhiteSpace(pageLink.ProviderName))
            {
                // Any search beneath a cloned page should in fact be performed under the original page, so we use a page link without any provider information
                pageLink = new PageReference(pageLink.ID);
            }

            PageDataCollection pages = DataFactory.Instance.FindPagesWithCriteria(
                pageLink, criterias, languageBranch, selector);

            // Return cloned search result set
            return new PageDataCollection(pages.Select(this.ClonePage));
        }

        /// <summary>
        /// The get descendent references.
        /// </summary>
        /// <param name="contentLink">
        /// The content link.
        /// </param>
        /// <returns>
        /// A <see cref="IList{ContentReference}"/> of descendents.
        /// </returns>
        public override IList<ContentReference> GetDescendentReferences(ContentReference contentLink)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                return new List<ContentReference>();
            }

            // If retrieving children for the entry point, we retrieve pages from the clone root
            contentLink = contentLink.CompareToIgnoreWorkID(this.EntryRoot) ? this.CloneRoot : contentLink;

            IList<ContentReference> descendents = this.ContentStore.ListAll(contentLink);

            foreach (ContentReference contentReference in
                descendents.Where(contentReference => !contentReference.CompareToIgnoreWorkID(this.EntryRoot)))
            {
                contentReference.ProviderName = this.ProviderKey;
            }

            return this.FilterByCategory(descendents);
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="config">
        /// The config.
        /// </param>
        public override void Initialize(string name, NameValueCollection config)
        {
            base.Initialize(name, config);

            this.CloneRoot = new PageReference(int.Parse(config.Get("root"), CultureInfo.InvariantCulture));
            this.EntryRoot = new PageReference(int.Parse(config.Get(PageProvider.EntryPointString), CultureInfo.InvariantCulture));
            this.providerKey = name;

            string[] categories = config.Get("categories").Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            this.Category = new CategoryList(Array.ConvertAll(categories, int.Parse));
        }

        #endregion

        #region Methods

        /// <summary>
        /// The construct content uri.
        /// </summary>
        /// <param name="contentTypeId">
        /// The content type id.
        /// </param>
        /// <param name="contentLink">
        /// The content link.
        /// </param>
        /// <param name="contentGuid">
        /// The content guid.
        /// </param>
        /// <returns>
        /// The <see cref="Uri"/> of the content.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// contentLink
        /// </exception>
        protected override Uri ConstructContentUri(int contentTypeId, ContentReference contentLink, Guid contentGuid)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                throw new ArgumentNullException("contentLink");
            }

            if (!contentLink.CompareToIgnoreWorkID(this.EntryRoot))
            {
                contentLink.ProviderName = this.ProviderKey;
            }

            return base.ConstructContentUri(contentTypeId, contentLink, contentGuid);
        }

        /// <summary>
        /// The load children references.
        /// </summary>
        /// <param name="contentLink">
        /// The content link.
        /// </param>
        /// <param name="languageID">
        /// The language id.
        /// </param>
        /// <param name="languageSpecific">
        /// The language specific.
        /// </param>
        /// <returns>
        /// An <see cref="IList{ContentReference}"/> of children.
        /// </returns>
        /// <remarks>
        /// If the provider supports structure this method should be implemented.
        /// </remarks>
        protected override IList<ContentReference> LoadChildrenReferences(
            ContentReference contentLink, string languageID, out bool languageSpecific)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                languageSpecific = false;
                return new List<ContentReference>();
            }

            // If retrieving children for the entry point, we retrieve pages from the clone root
            contentLink = contentLink.CompareToIgnoreWorkID(this.EntryRoot) ? this.CloneRoot : contentLink;

            FilterSortOrder sortOrder;

            IList<ContentReference> children = this.ContentStore.LoadChildrenReferences(
                contentLink.ID, languageID, out sortOrder);

            languageSpecific = sortOrder == FilterSortOrder.Alphabetical;

            foreach (ContentReference contentReference in
                children.Where(contentReference => !contentReference.CompareToIgnoreWorkID(this.EntryRoot)))
            {
                contentReference.ProviderName = this.ProviderKey;
            }

            return this.FilterByCategory(children);
        }

        /// <summary>
        /// Load the content for a <see cref="ContentReference"/>.
        /// </summary>
        /// <param name="contentLink">
        /// The content link.
        /// </param>
        /// <param name="languageSelector">
        /// The language selector.
        /// </param>
        /// <returns>
        /// The <see cref="IContent"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// contentLink
        /// </exception>
        /// <exception cref="System.NotSupportedException">
        /// Only cloning of pages is supported
        /// </exception>
        protected override IContent LoadContent(ContentReference contentLink, ILanguageSelector languageSelector)
        {
            if (ContentReference.IsNullOrEmpty(contentLink) || contentLink.ID == 0)
            {
                throw new ArgumentNullException("contentLink");
            }

            if (languageSelector == null)
            {
                languageSelector = LanguageSelector.AutoDetect();
            }

            if (contentLink.WorkID > 0)
            {
                return this.ContentStore.LoadVersion(contentLink, -1);
            }

            ILanguageBranchRepository languageBranchRepository =
                ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();

            LanguageSelectorContext context = new LanguageSelectorContext(
                contentLink, languageBranchRepository, this.Load);

            if (contentLink.GetPublishedOrLatest)
            {
                languageSelector.SelectPageLanguage(context);

                LanguageBranch langBr = null;

                if (context.SelectedLanguage != null)
                {
                    langBr = languageBranchRepository.Load(context.SelectedLanguage);
                }

                return this.ContentStore.LoadVersion(contentLink, langBr != null ? langBr.ID : -1);
            }

            languageSelector.SetInitializedLanguageBranch(context);

            // Get published version of Content
            IContent originalContent = this.ContentStore.Load(
                contentLink, context.SelectedLanguageBranch != null ? context.SelectedLanguageBranch.ID : -1);

            PageData page = originalContent as PageData;

            if (page == null)
            {
                throw new NotSupportedException("Only cloning of pages is supported");
            }

            return this.ClonePage(page);
        }

        /// <summary>
        /// The load contents.
        /// </summary>
        /// <param name="contentReferences">
        /// The content references.
        /// </param>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{IContent}"/> of cloned content.
        /// </returns>
        protected override IEnumerable<IContent> LoadContents(
            IList<ContentReference> contentReferences, ILanguageSelector selector)
        {
            IContentRepository contentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            return
                contentReferences.Select(
                    contentReference =>
                    this.ClonePage(contentRepository.Get<PageData>(new PageReference(contentReference.ID))))
                                 .Cast<IContent>()
                                 .ToList();
        }

        /// <summary>
        /// The resolve content.
        /// </summary>
        /// <param name="contentLink">
        /// The content link.
        /// </param>
        /// <returns>
        /// The <see cref="ContentResolveResult"/>.
        /// </returns>
        /// <remarks>
        /// This method should be implemented to support permanent link support. With permanent link  means that a link to a
        ///     <see cref="T:EPiServer.Core.IContent"/>
        ///     instance
        ///     can be stored in a Guid based format. Then the link is not broken even if content instance moves. Also permanent links are preserved during import/export and
        ///     mirroring.
        /// </remarks>
        protected override ContentResolveResult ResolveContent(ContentReference contentLink)
        {
            if (ContentReference.IsNullOrEmpty(contentLink))
            {
                return null;
            }

            ContentCoreData contentData = this.ContentCoreDataLoader.Service.Load(contentLink.ID);

            // All pages but the entry root should appear to come from this content provider
            if (!contentLink.CompareToIgnoreWorkID(this.EntryRoot))
            {
                contentData.ContentReference.ProviderName = this.ProviderKey;
            }

            ContentResolveResult result = this.CreateContentResolveResult(contentData);

            if (!result.ContentLink.CompareToIgnoreWorkID(this.EntryRoot))
            {
                result.ContentLink.ProviderName = this.ProviderKey;
            }

            return result;
        }

        /// <summary>
        /// The set cache settings.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        /// <param name="cacheSettings">
        /// The cache settings.
        /// </param>
        protected override void SetCacheSettings(IContent content, CacheSettings cacheSettings)
        {
            if (content == null || cacheSettings == null)
            {
                return;
            }

            // Make the cache of this content provider depend on the original content
            cacheSettings.CacheKeys.Add(
                DataFactoryCache.PageCommonCacheKey(new ContentReference(content.ContentLink.ID)));
        }

        /// <summary>
        /// The set cache settings.
        /// </summary>
        /// <param name="contentReference">
        /// The content reference.
        /// </param>
        /// <param name="children">
        /// The children.
        /// </param>
        /// <param name="cacheSettings">
        /// The cache settings.
        /// </param>
        protected override void SetCacheSettings(
            ContentReference contentReference, IEnumerable<ContentReference> children, CacheSettings cacheSettings)
        {
            if (ContentReference.IsNullOrEmpty(contentReference) || children == null || cacheSettings == null)
            {
                return;
            }

            // Make the cache of this content provider depend on the original content
            cacheSettings.CacheKeys.Add(DataFactoryCache.PageCommonCacheKey(new ContentReference(contentReference.ID)));

            foreach (ContentReference child in children)
            {
                cacheSettings.CacheKeys.Add(DataFactoryCache.PageCommonCacheKey(new ContentReference(child.ID)));
            }
        }

        /// <summary>
        /// Clones a page to make it appear to come from where the content provider is attached
        /// </summary>
        /// <param name="originalPage">
        /// The original Page.
        /// </param>
        /// <returns>
        /// The <see cref="PageData"/>.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">
        /// originalPage;No page to clone specified
        /// </exception>
        private PageData ClonePage(PageData originalPage)
        {
            if (originalPage == null)
            {
                throw new ArgumentNullException("originalPage", "No page to clone specified");
            }

            Logger.DebugFormat("Cloning page {0}...", originalPage.PageLink);

            PageData clone = originalPage.CreateWritableClone();

            // If original page was under the clone root, we make it appear to be under the entry root instead
            if (originalPage.ParentLink.CompareToIgnoreWorkID(this.CloneRoot))
            {
                clone.ParentLink = this.EntryRoot;
            }

            // All pages but the entry root should appear to come from this content provider
            if (!clone.PageLink.CompareToIgnoreWorkID(this.EntryRoot))
            {
                clone.ContentLink.ProviderName = this.ProviderKey;
            }

            // Unless the parent is the entry root, it should appear to come from this content provider
            if (!clone.ParentLink.CompareToIgnoreWorkID(this.EntryRoot))
            {
                PageReference parentLinkClone = clone.ParentLink.CreateWritableClone();

                parentLinkClone.ProviderName = this.ProviderKey;

                clone.ParentLink = parentLinkClone;
            }

            // This is integral to map the cloned page to this content provider
            clone.LinkURL =
                this.ConstructContentUri(originalPage.PageTypeID, clone.ContentLink, clone.ContentGuid).ToString();

            return clone;
        }

        /// <summary>
        /// Filters out content references to content that does not match current category filters, if any
        /// </summary>
        /// <param name="contentReferences">
        /// The content references.
        /// </param>
        /// <returns>
        /// A filtered <see cref="IList{ContentReference}"/>.
        /// </returns>
        private IList<ContentReference> FilterByCategory(IEnumerable<ContentReference> contentReferences)
        {
            if (this.Category == null || !this.Category.Any())
            {
                return contentReferences.ToList();
            }

            // Filter by category if a category filter has been set
            List<ContentReference> filteredChildren = new List<ContentReference>();

            foreach (ContentReference contentReference in contentReferences)
            {
                ICategorizable content = DataFactory.Instance.Get<IContent>(contentReference) as ICategorizable;

                if (content != null)
                {
                    bool atleastOneMatchingCategory = content.Category.Any(c => this.Category.Contains(c));

                    if (atleastOneMatchingCategory)
                    {
                        filteredChildren.Add(contentReference);
                    }
                }
                else
                {
                    // Non-categorizable content will also be included
                    filteredChildren.Add(contentReference);
                }
            }

            return filteredChildren;
        }

        #endregion
    }
}