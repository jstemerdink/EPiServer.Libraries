// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputHelper.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Web;

    using EPiServer.Core;
    using EPiServer.Libraries.Output.Formats;

    using ExCSS;
    using ExCSS.Model;

    using iTextSharp.text.html.simpleparser;

    using log4net;

    /// <summary>
    ///     Helpers for the format output.
    /// </summary>
    public static class OutputHelper
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Gets the enabled formats.
        /// </summary>
        /// <returns>A list of the enabled output formats.</returns>
        public static Collection<string> GetEnabledFormats()
        {
            Collection<string> enabledFormats = new Collection<string>();

            if (OutputSettings.Instance.EnablePDF)
            {
                enabledFormats.Add(OutputConstants.PDF);
            }

            if (OutputSettings.Instance.EnableJSON)
            {
                enabledFormats.Add(OutputConstants.JSON);
            }

            if (OutputSettings.Instance.EnableTXT)
            {
                enabledFormats.Add(OutputConstants.Text);
            }

            if (OutputSettings.Instance.EnableXML)
            {
                enabledFormats.Add(OutputConstants.XML);
            }

            return enabledFormats;
        }

        /// <summary> Gets the CSS classes. </summary>
        /// <returns> The CSS classes. </returns>
        public static StyleSheet GetStyleSheet()
        {
            string pdfStyleSheet = OutputSettings.Instance.StyleSheet;

            if (string.IsNullOrWhiteSpace(pdfStyleSheet))
            {
                return null;
            }

            StylesheetParser ssp = new StylesheetParser();
            string css = File.ReadAllText(HttpContext.Current.Server.MapPath(pdfStyleSheet));
            Stylesheet stylesheet = ssp.Parse(css);

            StyleSheet styles = new StyleSheet();

            foreach (RuleSet ruleSet in stylesheet.RuleSets)
            {
                Dictionary<string, string> attributes =
                    ruleSet.Declarations.ToDictionary(
                        declaration => declaration.Name,
                        declaration => declaration.Expression.ToString());

                foreach (Selector selector in ruleSet.Selectors)
                {
                    if (selector.ToString().StartsWith(".", StringComparison.OrdinalIgnoreCase))
                    {
                        styles.LoadStyle(selector.ToString(), attributes);
                    }
                    else
                    {
                        styles.LoadTagStyle(selector.ToString(), attributes);
                    }
                }
            }

            return styles;
        }

        /// <summary>
        /// Determines whether [is valid request] [the specified page].
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        /// <returns>
        ///   <c>true</c> if [is valid request] [the specified page]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidRequest(PageData page, HttpContextBase context, ILog logger)
        {
            if (page == null)
            {
                if (logger == null)
                {
                    return false;
                }

                bool requestIsNull = context == null || context.Request == null || context.Request.Url == null;

                logger.InfoFormat(
                    CultureInfo.InvariantCulture,
                    "[OutputFormats] PageData is null for: '{0}'.",
                    requestIsNull ? string.Empty : context.Request.Url.ToString());

                return false;
            }

            if (context == null)
            {
                if (logger == null)
                {
                    return false;
                }

                logger.InfoFormat(
                    CultureInfo.InvariantCulture,
                    "[OutputFormats] Context is not available for: '{0}'.",
                    page.Name);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Handles the json format.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="httpContextBase">The HttpContext</param>
        public static void HandleJson(PageData currentPage, HttpContextBase httpContextBase = null)
        {
            if (currentPage.CannotBeUsedInOutput())
            {
                return;
            }

            JsonOutputFormat jsonOutputFormat = new JsonOutputFormat();
            jsonOutputFormat.HandleFormat(currentPage, httpContextBase ?? new HttpContextWrapper(HttpContext.Current));
        }

        /// <summary>
        /// Handles the txt format.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="httpContextBase">The HttpContext</param>
        public static void HandleTxt(PageData currentPage, HttpContextBase httpContextBase = null)
        {
            if (currentPage.CannotBeUsedInOutput())
            {
                return;
            }

            TxtOutputFormat txtOutputFormat = new TxtOutputFormat();
            txtOutputFormat.HandleFormat(currentPage, httpContextBase ?? new HttpContextWrapper(HttpContext.Current));
        }

        /// <summary>
        /// Handles the xml format.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="httpContextBase">The HttpContext</param>
        public static void HandleXml(PageData currentPage, HttpContextBase httpContextBase = null)
        {
            if (currentPage.CannotBeUsedInOutput())
            {
                return;
            }

            XmlOutputFormat xmlOutputFormat = new XmlOutputFormat();
            xmlOutputFormat.HandleFormat(currentPage, httpContextBase ?? new HttpContextWrapper(HttpContext.Current));
        }

        /// <summary>
        /// Handles the pdf format.
        /// </summary>
        /// <param name="currentPage">The current page.</param>
        /// <param name="httpContextBase">The HttpContext</param>
        public static void HandlePdf(PageData currentPage, HttpContextBase httpContextBase = null)
        {
            if (currentPage.CannotBeUsedInOutput())
            {
                return;
            }

            PdfOutputFormat pdfOutputFormat = new PdfOutputFormat();
            pdfOutputFormat.HandleFormat(currentPage, httpContextBase ?? new HttpContextWrapper(HttpContext.Current));
        }

        #endregion
    }
}