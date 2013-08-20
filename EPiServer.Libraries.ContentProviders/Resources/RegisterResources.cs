// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisterResources.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders.Resources
{
    using System.Web;
    using System.Web.Hosting;

    using EPiServer.PlugIn;

    /// <summary>
    /// The register resources.
    /// </summary>
    internal sealed class RegisterResources : PlugInAttribute
    {
        #region Public Methods and Operators

        /// <summary>
        /// The start.
        /// </summary>
        public static void Start()
        {
            RegisterAdminResources();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The register vpp resources.
        /// </summary>
        private static void RegisterAdminResources()
        {
            if (HttpContext.Current == null)
            {
                return;
            }

            HostingEnvironment.RegisterVirtualPathProvider(
            new ResourcePathProvider(
                "/util/settings/ContentProviderAdministration.aspx",
                "EPiServer.Libraries.ContentProviders.Plugins.ContentProviderAdministration.aspx",
                false));
        }

        #endregion
    }
}