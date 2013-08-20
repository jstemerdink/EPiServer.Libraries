// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmbeddedResourceLoader.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders.Resources
{
    using System;
    using System.IO;
    using System.Reflection;

    /// <summary>
    ///     Used to read embedded resources from the eFocus Libraries assembly
    /// </summary>
    public static class EmbeddedResourceLoader
    {
        #region Constants

        /// <summary>
        ///     Assembly cannot be null.
        /// </summary>
        private const string AssemblyCannotBeNull = "Assembly cannot be null.";

        /// <summary>
        ///     Resourcename cannot be empty.
        /// </summary>
        private const string ResourcenameCannotBeEmpty = "Resourcename cannot be empty.";

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the requested resource from the specified assembly
        /// </summary>
        /// <param name="resource">
        /// Resource identifier without the assembly name
        /// </param>
        /// <param name="assembly">
        /// The assembly.
        /// </param>
        /// <returns>
        /// The content from the resource.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Resourcename cannot be empty.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// Assembly cannot be null.
        /// </exception>
        public static Stream LoadResource(string resource, Assembly assembly)
        {
            if (string.IsNullOrWhiteSpace(resource))
            {
                throw new ArgumentException(ResourcenameCannotBeEmpty, "resource");
            }

            if (assembly == null)
            {
                throw new ArgumentNullException("assembly", AssemblyCannotBeNull);
            }

            resource = resource.Replace('/', '.');

            if (resource.StartsWith(".", StringComparison.OrdinalIgnoreCase))
            {
                resource = resource.Substring(1);
            }

            string assemblyName = assembly.ManifestModule.Name.Replace(".DLL", string.Empty)
                .Replace(".dll", string.Empty);

            return assembly.GetManifestResourceStream(string.Concat(assemblyName, ".", resource));
        }

        #endregion
    }
}