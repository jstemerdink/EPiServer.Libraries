// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceVirtualFile.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders.Resources
{
    using System.IO;
    using System.Reflection;
    using System.Web.Hosting;

    /// <summary>
    /// The resource virtual file.
    /// </summary>
    internal class ResourceVirtualFile : VirtualFile
    {
        #region Constants and Fields

        /// <summary>
        /// The _physical resource.
        /// </summary>
        private readonly bool physicalResource;

        /// <summary>
        /// The _resource name.
        /// </summary>
        private readonly string resourceName;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVirtualFile"/> class.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <param name="resourceName">The resource name.</param>
        /// <param name="physicalResource">The physical resource.</param>
        public ResourceVirtualFile(string virtualPath, string resourceName, bool physicalResource)
            : base(virtualPath)
        {
            this.resourceName = resourceName;
            this.physicalResource = physicalResource;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The open.
        /// </summary>
        /// <returns>
        /// A read-only stream to the virtual file.
        /// </returns>
        public override Stream Open()
        {
            return this.physicalResource ? File.OpenRead(this.resourceName) : Assembly.GetExecutingAssembly().GetManifestResourceStream(this.resourceName);
        }

        #endregion
    }
}