// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EPiServerEpiServerCacheBaseAttribute.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   CacheAttribute.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace EPiServer.Libraries.Aspects.Caching
{
    using System;
    using System.Web.Caching;

    using PostSharp.Extensibility;

    /// <summary>A cache attribute. Skip the execution of a method when its value is found in the EPiServer cache.</summary>
    /// <example>
    /// [assembly: Cache( AttributeTargetTypes="AdventureWorks.BusinessLayer.*", AttributeTargetMemberAttributes = MulticastAttributes.Public )]
    /// </example>
    [MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Public)]
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    [Serializable]
    public sealed class EPiServerEpiServerCacheBaseAttribute : EpiServerCacheBaseAttribute
    {
        /// <summary>
        /// Gets the Cache Dependency
        /// </summary>
        public override CacheDependency Dependency
        {
            get
            {
                return new CacheDependency(null, new[] { DataFactoryCache.VersionKey });
            }
        }
     }
}