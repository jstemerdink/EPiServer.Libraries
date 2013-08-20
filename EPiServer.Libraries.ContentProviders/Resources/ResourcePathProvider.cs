// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourcePathProvider.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders.Resources
{
    using System;
    using System.Collections;
    using System.Web.Caching;
    using System.Web.Hosting;

    /// <summary>
    /// The resource path provider.
    /// </summary>
    public class ResourcePathProvider : VirtualPathProvider
    {
        #region Constants and Fields

        /// <summary>
        /// The physical resource.
        /// </summary>
        private readonly bool physicalResource;

        /// <summary>
        /// The resource name.
        /// </summary>
        private readonly string resourceName;

        /// <summary>
        /// The virtual path.
        /// </summary>
        private readonly string virtualFilePath;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourcePathProvider"/> class.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        /// <param name="physicalResource">
        /// The physical resource.
        /// </param>
        public ResourcePathProvider(string virtualPath, string resourceName, bool physicalResource)
        {
            this.virtualFilePath = virtualPath;
            this.resourceName = resourceName;
            this.physicalResource = physicalResource;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Checks wether the file exists.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>
        /// The file exists.
        /// </returns>
        public override bool FileExists(string virtualPath)
        {
            return string.Compare(virtualPath, this.virtualFilePath, StringComparison.OrdinalIgnoreCase) == 0 || this.Previous.FileExists(virtualPath);
        }

        /// <summary>
        /// Get the file.
        /// </summary>
        /// <param name="virtualPath">
        /// The virtual path.
        /// </param>
        /// <returns>
        /// The file.
        /// </returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            return string.Compare(virtualPath, this.virtualFilePath, StringComparison.OrdinalIgnoreCase) != 0 
                ? this.Previous.GetFile(virtualPath) 
                : new ResourceVirtualFile(this.virtualFilePath, this.resourceName, this.physicalResource);
        }

        /// <summary> Creates a cache dependency based on the specified virtual paths. </summary>
        /// <param name="virtualPath">             The path to the primary virtual resource. </param>
        /// <param name="virtualPathDependencies">
        ///     An array of paths to other resources required by the primary virtual resource.
        /// </param>
        /// <param name="utcStart">
        ///     The UTC time at which the virtual resources were read.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Web.Caching.CacheDependency" /> object for the specified virtual
        ///     resources.
        /// </returns>
        /// <seealso cref="System.Web.Hosting.VirtualPathProvider.GetCacheDependency(string,IEnumerable,DateTime)"/>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            try
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}