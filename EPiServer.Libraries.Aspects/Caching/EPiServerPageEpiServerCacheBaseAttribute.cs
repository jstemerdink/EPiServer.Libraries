// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EPiServerPageEpiServerCacheBaseAttribute.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   CacheAttribute.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace EPiServer.Libraries.Aspects.Caching
{
    using System;
    using System.Web;
    using System.Web.Caching;

    /// <summary>A cache attribute. Skip the execution of a method when its value is found in the EPiServer cache for the current page.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [Serializable]
    public sealed class EPiServerPageEpiServerCacheBaseAttribute : EpiServerCacheBaseAttribute
    {
        /// <summary>
        /// Gets the Cache Dependency
        /// </summary>
        public override CacheDependency Dependency
        {
            get
            {
                PageBase page = HttpContext.Current.Handler as PageBase;

                return page == null ? null : DataFactoryCache.CreateDependency(page.CurrentPage.PageLink);
            }
        }
    }
}