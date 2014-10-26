using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPi.Libraries.Localization
{
    /// <summary>
    /// Interface ITranslationService
    /// </summary>
    public interface ITranslationService
    {
        /// <summary>
        /// Translates the specified text.
        /// </summary>
        /// <param name="toBeTranslated">The text to translate.</param>
        /// <param name="fromLang">From language.</param>
        /// <param name="toLang">To language.</param>
        /// <returns>System.String.</returns>
        string Translate(string toBeTranslated, string fromLang, string toLang);
    }
}
