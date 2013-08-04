// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UseInOutputAttribute.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   Attribute to indicate whether a property should be used in text/pdf output.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.DataAnnotations
{
    using System;

    /// <summary> Attribute to indicate whether a property should be used in text/pdf output. </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class UseInOutputAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="UseInOutputAttribute" /> class.
        /// </summary>
        public UseInOutputAttribute()
        {
            this.UseInOutput = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UseInOutputAttribute"/> class.
        /// </summary>
        /// <param name="useInOutput">
        /// The use in channel.
        /// </param>
        public UseInOutputAttribute(bool useInOutput)
        {
            this.UseInOutput = useInOutput;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets a value indicating whether use in channel.
        /// </summary>
        public bool UseInOutput { get; private set; }

        #endregion
    }
}