// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageProvider.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   Image provider to add host to relative images..
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Hosting;

    using EPiServer.Configuration;
    using EPiServer.Web.Hosting;

    using iTextSharp.text;
    using iTextSharp.text.html.simpleparser;

    using log4net;

    /// <summary>
    ///     Image provider to add host to relative images..
    /// </summary>
    public class ImageProvider : IImageProvider
    {
        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="ImageProvider" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ImageProvider));

        // Store a reference to the main document so that we can access the page size and margins
        #region Fields

        /// <summary>The main doc.</summary>
        private readonly Document mainDoc;

        #endregion

        // Constructor
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageProvider"/> class.
        /// </summary>
        /// <param name="doc">
        /// The doc.
        /// </param>
        public ImageProvider(Document doc)
        {
            this.mainDoc = doc;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get image.
        /// </summary>
        /// <param name="src">The src.</param>
        /// <param name="attrs">The attrs.</param>
        /// <param name="chain">The chain.</param>
        /// <param name="doc">The doc.</param>
        /// <returns>
        /// The <see cref="Image" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">src</exception>
        /// <exception cref="ArgumentNullException">src</exception>
        public Image GetImage(string src, IDictionary<string, string> attrs, ChainedProperties chain, IDocListener doc)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return null;
            }

            if (src.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                return this.GetImage(src);
            }

            if (HostingEnvironment.VirtualPathProvider.FileExists(src))
            {
                try
                {
                    UnifiedFile unifiedFile = HostingEnvironment.VirtualPathProvider.GetFile(src) as UnifiedFile;
                    return unifiedFile == null ? null : this.GetImage(unifiedFile.LocalPath);
                }
                catch (Exception exception)
                {
                    Logger.ErrorFormat(CultureInfo.InvariantCulture, "[OutputFormats] File not found for:'{0}'. \n {1}", src, exception);
                    return null;
                }
            }

            string baseurl = Settings.Instance.SiteUrl.GetLeftPart(UriPartial.Authority);
            src = string.Format(CultureInfo.InvariantCulture, "{0}{1}", baseurl, src);

            return this.GetImage(src);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the image.
        /// </summary>
        /// <param name="src">
        /// The image path.
        /// </param>
        /// <returns>
        /// The image.
        /// </returns>
        private Image GetImage(string src)
        {
            Image img;

            try
            {
                img = Image.GetInstance(src);
            }
            catch
            {
                return null;
            }

            // Make sure we got something
            if (img == null)
            {
                return null;
            }

            // Determine the usable area of the canvas. NOTE, this doesn't take into account the current "cursor" position so this might create a new blank page just for the image
            float usableW = this.mainDoc.PageSize.Width - (this.mainDoc.LeftMargin + this.mainDoc.RightMargin);
            float usableH = this.mainDoc.PageSize.Height - (this.mainDoc.TopMargin + this.mainDoc.BottomMargin);

            // If the downloaded image is bigger than either width and/or height then shrink it
            if (img.Width > usableW || img.Height > usableH)
            {
                img.ScaleToFit(usableW, usableH);
            }

            if (img.Alt == null)
            {
                img.Alt = string.Empty;
            }

            // return our image
            return img;
        }

        #endregion
    }
}