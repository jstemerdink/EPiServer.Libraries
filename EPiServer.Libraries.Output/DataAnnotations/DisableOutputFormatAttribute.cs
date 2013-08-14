// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableOutputFormatAttribute.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.DataAnnotations
{
    using System;

    /// <summary>
    /// The disable output format attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DisableOutputFormatAttribute : Attribute
    {
        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether to allow the page type to be used in the output.
        /// </summary>
        public static bool DisableOutput
        {
            get
            {
                return true;
            }
        }

        #endregion
    }
}