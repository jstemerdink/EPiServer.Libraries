// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.DataAnnotations;

    using HtmlAgilityPack;

    /// <summary>
    ///     The extensions.
    /// </summary>
    internal static class Extensions
    {
        #region Public Methods and Operators

        /// <summary>
        /// A MemberInfo extension method that gets a page type property attribute.
        /// </summary>
        /// <param name="self">
        /// The member to act on.
        /// </param>
        /// <returns>
        /// The page type property attribute.
        /// </returns>
        public static UseInOutputAttribute GetUseInOutputAttribute(this PropertyInfo self)
        {
            return (UseInOutputAttribute)Attribute.GetCustomAttribute(self, typeof(UseInOutputAttribute));
        }

        /// <summary>
        /// Gets the order value.
        /// </summary>
        /// <param name="otputAttribute">
        /// The otput attribute.
        /// </param>
        /// <returns>
        /// The order of the property.
        /// </returns>
        public static int GetOrderValue(this UseInOutputAttribute otputAttribute)
        {
            return otputAttribute == null ? 0 : otputAttribute.Order;
        }

        /// <summary>
        /// Gets the properties that are marked to use in the output.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// A <see cref="List{PropertyInfo}"/>
        /// </returns>
        public static List<KeyValuePair<string, string>> GetPropertyValues(this IContent page)
        {
            List<KeyValuePair<string, string>> propertyValues = new List<KeyValuePair<string, string>>();

            foreach (PropertyInfo propertyInfo in
                page.GetType()
                    .GetProperties()
                   .Where(propertyInfo => propertyInfo.HasAttribute<UseInOutputAttribute>())
                    .OrderBy(p => p.GetUseInOutputAttribute().GetOrderValue()))
            {
                if (propertyInfo.PropertyType != typeof(ContentArea))
                {
                    object value = propertyInfo.GetValue(page, null);

                    if (value != null)
                    {
                        string objectValue = value.ToString();

                        if (!string.IsNullOrWhiteSpace(objectValue))
                        {
                            propertyValues.Add(new KeyValuePair<string, string>(propertyInfo.Name, objectValue));
                        }
                    }

                    continue;
                }

                ContentArea contentArea = propertyInfo.GetValue(page, null) as ContentArea;
                propertyValues.AddRange(contentArea.Contents());
            }

            return propertyValues;
        }

        /// <summary>
        /// Determines whether the specified self has attribute.
        /// </summary>
        /// <typeparam name="T">
        /// Generic type.
        /// </typeparam>
        /// <param name="self">
        /// The self.
        /// </param>
        /// <returns>
        /// <c>true</c> if the specified self has attribute; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasAttribute<T>(this PropertyInfo self) where T : Attribute
        {
            T attr = (T)Attribute.GetCustomAttribute(self, typeof(T));

            return attr != null;
        }

        /// <summary>
        /// A string extension method that strip tags.
        /// </summary>
        /// <param name="html">
        /// The HTML to clean.
        /// </param>
        /// <returns>
        /// The stripped hmtl.
        /// </returns>
        public static string StripTags(this string html)
        {
            HtmlDocument doc = new HtmlDocument { OptionFixNestedTags = true };
            doc.LoadHtml(html.TrimWhiteSpace());

            // Strip the script tags
            HtmlNodeCollection scripts = doc.DocumentNode.SelectNodes("//script");

            if (scripts != null)
            {
                foreach (HtmlNode tag in scripts)
                {
                    tag.Remove();
                }
            }

            // Strip horizontal rules, iTextSharp chokes on those.
            HtmlNodeCollection rules = doc.DocumentNode.SelectNodes("//hr");

            if (rules != null)
            {
                foreach (HtmlNode tag in rules)
                {
                    tag.Remove();
                }
            }

            // Replace links with the link text.
            HtmlNodeCollection links = doc.DocumentNode.SelectNodes("//a");

            if (links != null)
            {
                foreach (HtmlNode tag in links)
                {
                    string linkText = tag.InnerText;
                    HtmlNode nodeForReplace = HtmlNode.CreateNode(linkText);
                    tag.ParentNode.ReplaceChild(nodeForReplace, tag);
                }
            }

            string parsedHTML = doc.DocumentNode.WriteTo();
            return string.IsNullOrWhiteSpace(parsedHTML) ? html : parsedHTML;
        }

        /// <summary>
        /// Trims whitespaces including non printing
        ///     whitespaces like carriage returns, line feeds,
        ///     and form feeds.
        /// </summary>
        /// <param name="stringToTrim">
        /// The string to trim
        /// </param>
        /// <returns>
        /// The trimmed string.
        /// </returns>
        public static string TrimWhiteSpace(this string stringToTrim)
        {
            if (stringToTrim == null)
            {
                return null;
            }

            char[] whiteSpace = { '\r', '\n', '\f', '\t', '\v' };
            return stringToTrim.Trim(whiteSpace).Trim();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the properties of the content in the specified content area.
        /// </summary>
        /// <param name="contentArea">
        /// The content area.
        /// </param>
        /// <returns>
        /// A <see cref="List{T}"/>
        /// </returns>
        private static IEnumerable<KeyValuePair<string, string>> Contents(this ContentArea contentArea)
        {
            List<KeyValuePair<string, string>> propertyValues = new List<KeyValuePair<string, string>>();

            if (contentArea == null)
            {
                return propertyValues;
            }

            foreach (IContent content in contentArea.Contents)
            {
                propertyValues.AddRange(content.GetPropertyValues());
            }

            return propertyValues;
        }

        #endregion
    }
}