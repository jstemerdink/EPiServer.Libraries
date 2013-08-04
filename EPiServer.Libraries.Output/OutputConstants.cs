// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputConstants.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   The rendering tags.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output
{
    /// <summary>
    ///     The rendering tags.
    /// </summary>
    public static class OutputConstants
    {
        #region Constants

        /// <summary>
        /// The PDF Content-Type header.
        /// </summary>
        public const string ApplicationPDF = "application/pdf";

        /// <summary>
        ///     Render as JSON.
        /// </summary>
        public const string JSON = "JSON";

        /// <summary>
        /// The JSON Content-Type header.
        /// </summary>
        public const string ApplicationJSON = "application/json";

        /// <summary>
        ///     Render as PDF.
        /// </summary>
        public const string PDF = "PDF";

        /// <summary>
        ///     Render as text.
        /// </summary>
        public const string Text = "TXT";

        /// <summary>
        ///     Render as XML.
        /// </summary>
        public const string XML = "XML";

        /// <summary>
        /// The XML Content-Type header.
        /// </summary>
        public const string TextXML = "text/xml";
        
        /// <summary>
        /// The TXT Content-Type header.
        /// </summary>
        public const string TextPlain = "text/plain";

        #endregion
    }
}