// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiConfig.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.WebApi.Output
{
    using System.Web.Http;

    /// <summary>
    ///     The web api config.
    /// </summary>
    public static class WebApiConfig
    {
        #region Public Methods and Operators

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public static void Register(HttpConfiguration config)
        {
            if (config == null)
            {
                return;
            }

            config.Routes.MapHttpRoute(
                name: "ContentApi",
                routeTemplate: "api/{controller}/{id}/{language}",
                defaults: new { language = RouteParameter.Optional });
        }

        #endregion
    }
}