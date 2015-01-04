// Copyright© 2014 Jeroen Stemerdink. All Rights Reserved.
// 
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

using EPiServer.Framework.Localization.XmlResources;

using log4net;
using EPiServer.Framework.Localization;

namespace EPiServer.Libraries.Localization
{
    /// <summary>
    ///     The translation provider.
    /// </summary>
    public class TranslationProvider : LocalizationProvider
    {
        #region Public Properties

        /// <summary>
        ///     Gets all available languages from the translation container.
        ///     An available language does not need to contain any translations.
        /// </summary>
        public override IEnumerable<CultureInfo> AvailableLanguages
        {
            get
            {
                return TranslationFactory.Instance.LocalizationProvider.AvailableLanguages;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets a translated string from a language key.
        /// </summary>
        /// <param name="originalKey">The unmodified key</param>
        /// <param name="normalizedKey">The <paramref name="originalKey" /> normalized and split into an array</param>
        /// <param name="culture">The requested culture for the resource string</param>
        /// <returns>A translated resource string</returns>
        public override string GetString(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return TranslationFactory.Instance.LocalizationProvider.GetString(originalKey, normalizedKey, culture);
        }

        /// <summary>
        /// Gets all localized strings for a specific culture. Will return all strings below the specified key.
        /// </summary>
        /// <param name="originalKey">The unmodified key</param>
        /// <param name="normalizedKey">The <paramref name="originalKey" /> normalized and split into an array</param>
        /// <param name="culture">The requested culture for the resource string</param>
        /// <returns>
        /// All resource strings below the specified key
        /// </returns>
        public override IEnumerable<ResourceItem> GetAllStrings(string originalKey, string[] normalizedKey, CultureInfo culture)
        {
            return TranslationFactory.Instance.LocalizationProvider.GetAllStrings(originalKey, normalizedKey, culture);
        }

        #endregion
    }
}