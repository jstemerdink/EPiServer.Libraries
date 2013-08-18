// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentController.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.WebApi.Output.Controllers
{
    using System.Net;
    using System.Web;
    using System.Web.Http;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.Formats;
    using EPiServer.Libraries.Output.Helpers;
    using EPiServer.ServiceLocation;

    using log4net;

    /// <summary>
    ///     The content controller
    /// </summary>
    public class ContentController : ApiController
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="ContentController" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ContentController));

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ContentController" /> class.
        /// </summary>
        public ContentController()
        {
            this.ContentRepository = ServiceLocator.Current.GetInstance<IContentRepository>();
            this.CurrentHttpContext = new HttpContextWrapper(HttpContext.Current);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentController"/> class.
        /// </summary>
        /// <param name="contentRepository">
        /// The content repository.
        /// </param>
        /// <param name="httpContextBase">The HttpContext</param>
        public ContentController(IContentRepository contentRepository, HttpContextBase httpContextBase)
        {
            this.ContentRepository = contentRepository;
            this.CurrentHttpContext = httpContextBase;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the content repository.
        /// </summary>
        /// <value>
        ///     The content repository.
        /// </value>
        private IContentRepository ContentRepository { get; set; }

        /// <summary>
        /// Gets or sets the HTTP context base.
        /// </summary>
        /// <value>
        /// The HTTP context base.
        /// </value>
        private HttpContextBase CurrentHttpContext { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Gets the content with the specified PageID.
        /// </summary>
        /// <param name="id">
        /// The PageID.
        /// </param>
        /// <param name="language">
        /// The language selector
        /// </param>
        /// <returns>
        /// The requested content
        /// </returns>
        /// <exception cref="System.Web.Http.HttpResponseException">
        /// A response exception.
        /// </exception>
        public string Get(int id, string language)
        {
            PageData requestedPageData = null;

            try
            {
                requestedPageData = this.ContentRepository.Get<PageData>(
                    new ContentReference(id), 
                    language == null ? LanguageSelector.AutoDetect() : new LanguageSelector(language));
            }
            catch (ContentNotFoundException contentNotFoundException)
            {
                Logger.Info(contentNotFoundException.Message, contentNotFoundException);
            }

            if (requestedPageData == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return this.GenerateResponse(requestedPageData);
        }

        /// <summary>
        /// Gets the content with the specified PageID.
        /// </summary>
        /// <param name="id">
        /// The PageID.
        /// </param>
        /// <returns>
        /// The requested content.
        /// </returns>
        public string Get(int id)
        {
            return this.Get(id, null);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates the response.
        /// </summary>
        /// <param name="requestedPageData">
        /// The requested page data.
        /// </param>
        /// <returns>
        /// The requested content.
        /// </returns>
        /// <exception cref="System.Web.Http.HttpResponseException">
        /// A response exception.
        /// </exception>
        private string GenerateResponse(PageData requestedPageData)
        {
            if (this.CurrentHttpContext.IsJsonAccepted())
            {
                if (WebApiSettings.Instance.EnableJSON)
                {
                    return JsonOutputFormat.GenerateJson(requestedPageData);
                }

                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            if (this.CurrentHttpContext.IsXmlAccepted())
            {
                if (WebApiSettings.Instance.EnableXML)
                {
                    return XmlOutputFormat.GenerateXml(requestedPageData);
                }

                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            throw new HttpResponseException(HttpStatusCode.BadRequest);
        }

        #endregion
    }
}