using System;

namespace EPiServer.Libraries.SEO.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class KeywordsMetaTagAttribute : Attribute
    {
        /// <summary>
        ///     Gets a value indicating whether the property is used for KeyWords.
        /// </summary>
        public static bool KeywordsMetaTag
        {
            get
            {
                return true;
            }
        }
    }
}
