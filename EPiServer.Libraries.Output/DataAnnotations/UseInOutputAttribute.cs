// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseInOutputAttribute.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.DataAnnotations
{
    using System;

    /// <summary> Attribute to indicate whether a property should be used in text/pdf output. </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UseInOutputAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether to use the property in the output.
        /// </summary>
        public static bool UseInOutput
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///     Gets or sets the order to render the property in.
        /// </summary>
        public int Order { get; set; }

        #endregion
    }
}