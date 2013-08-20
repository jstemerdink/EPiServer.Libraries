// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClonedContentProviderSettings.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders.Models
{
    using EPiServer.Data;
    using EPiServer.Data.Dynamic;

    /// <summary>
    /// Setttings for the Cloned Content Provider
    /// </summary>
    [EPiServerDataStore(AutomaticallyCreateStore = true, AutomaticallyRemapStore = true)]
    public class ClonedContentProviderSettings
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ClonedContentProviderSettings" /> class.
        /// </summary>
        public ClonedContentProviderSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClonedContentProviderSettings" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="entryPoint">The entry point.</param>
        /// <param name="root">The root.</param>
        /// <param name="categories">The categories.</param>
        public ClonedContentProviderSettings(string name, int entryPoint, int root, string categories)
        {
            this.Name = name;
            this.EntryPoint = entryPoint;
            this.Root = root;
            this.CategoryList = categories;
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the categories.</summary>
        public string CategoryList { get; set; }

        /// <summary> Gets or sets the id.</summary>
        public Identity Id { get; set; }

        /// <summary>Gets or sets the entry point.</summary>
        public int EntryPoint { get; set; }

        /// <summary>Gets or sets the name.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the root.</summary>
        public int Root { get; set; }

        #endregion
    }
}