// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputFormatAttribute.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.DataAnnotations
{
    using System;

    /// <summary>The output format plugin attribute.</summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class OutputFormatAttribute : Attribute
    {
        #region Fields

        /// <summary>
        /// The accepted format.
        /// </summary>
        private readonly string acceptedFormat;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputFormatAttribute"/> class.
        /// </summary>
        /// <param name="acceptedFormat">
        /// The accepted formats.
        /// </param>
        public OutputFormatAttribute(string acceptedFormat)
        {
            this.acceptedFormat = acceptedFormat;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets the accepted format.</summary>
        public string AcceptedFormat
        {
            get
            {
                return this.acceptedFormat;
            }
        }

        #endregion
    }
}