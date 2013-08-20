// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ContentProviderAdministration.aspx.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.ContentProviders.Plugins
{
    using System;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;

    using EPiServer.Core;
    using EPiServer.Libraries.ContentProviders.Helpers;
    using EPiServer.Libraries.ContentProviders.Models;
    using EPiServer.PlugIn;
    using EPiServer.Security;
    using EPiServer.ServiceLocation;
    using EPiServer.UI;
    using EPiServer.Web.WebControls;

    /// <summary>
    ///     The virtual page providers administration.
    /// </summary>
    [GuiPlugIn(Area = PlugInArea.AdminMenu, DisplayName = "Administrate Contentproviders", 
        RequiredAccess = AccessLevel.Administer, Url = "~/util/settings/ContentProviderAdministration.aspx")]
    public partial class ContentProviderAdministration : SystemPageBase, ICustomPlugInLoader
    {
        #region Fields

        /// <summary>
        ///     The providers.
        /// </summary>
        private Collection<ClonedContentProviderSettings> providerSettings = new Collection<ClonedContentProviderSettings>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Gets the PlugInDescriptor.
        /// </summary>
        /// <returns> The plugin descriptor </returns>
        /// <seealso cref="ICustomPlugInLoader.List()" />
        public PlugInDescriptor[] List()
        {
            return (!PrincipalInfo.HasAdminAccess)
                       ? new PlugInDescriptor[] { }
                       : new[] { PlugInDescriptor.Load(this.GetType()) };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Adds the provider.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="EventArgs"/> instance containing the event data.
        /// </param>
        protected void AddProvider(object sender, EventArgs e)
        {
            InputPageReference root = (InputPageReference)this.listView.FindControl("rootPage");
            InputPageReference entryPoint = (InputPageReference)this.listView.FindControl("entryPoint");

            PageData rootPage = ServiceLocator.Current.GetInstance<IContentRepository>().Get<PageData>(root.PageLink);
            PageData entryPage =
                ServiceLocator.Current.GetInstance<IContentRepository>().Get<PageData>(entryPoint.PageLink);

            string pageName = Regex.Replace(rootPage.Name, "[^a-zA-Z0-9]+", string.Empty, RegexOptions.Compiled);
            
            try
            {
                ClonedContentProviderHelpers.AddProvider(root.PageLink.ID, entryPoint.PageLink.ID, pageName, entryPage.Category);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ShowError(invalidOperationException.Message);
            }

            this.BindData();
        }

        /// <summary>
        /// Deletings the item.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="ListViewDeleteEventArgs"/> instance containing the event data.
        /// </param>
        protected void DeletingItem(object sender, ListViewDeleteEventArgs e)
        {
            Label labelName = this.listView.Items[e.ItemIndex].FindControl("labelName") as Label;

            if (labelName == null)
            {
                return;
            }

            try
            {
                ClonedContentProviderHelpers.DeleteProvider(labelName.Text);
                this.listView.Items.RemoveAt(e.ItemIndex);
            }
            catch (InvalidOperationException invalidOperationException)
            {
                this.ShowError(invalidOperationException.Message);
            }

            this.BindData();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Page.PreInit"/> event at the beginning of page initialization.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.EventArgs"/> that contains the event data.
        /// </param>
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.MasterPageFile = this.ResolveUrlFromUI("MasterPages/EPiServerUI.master");

            this.SystemMessageContainer.Heading = "Manage content providers";
            this.SystemMessageContainer.Description =
                "Select 'Delete' on the ContentProvider to delete the provider from the DataStore. Select 'Add' to add a new provider";
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load(object sender, EventArgs e)
        {
            this.BindData();
        }

        /// <summary>
        /// The row data bound.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void RowDataBound(object sender, ListViewItemEventArgs e)
        {
            ListViewDataItem dataItem = (ListViewDataItem)e.Item;

            if (e.Item.ItemType != ListViewItemType.DataItem)
            {
                return;
            }

            Label labelName = (Label)e.Item.FindControl("labelName");
            Label labelRoot = (Label)e.Item.FindControl("labelRoot");
            Label labelEntryPoint = (Label)e.Item.FindControl("labelEntryPoint");
            Button buttonDelete = (Button)e.Item.FindControl("buttonDelete");
            HtmlGenericControl cmsButtonDelete = (HtmlGenericControl)e.Item.FindControl("cmsButtonDelete");

            ClonedContentProviderSettings pageProviderSettings = dataItem.DataItem as ClonedContentProviderSettings;

            if (pageProviderSettings == null)
            {
                return;
            }

            labelName.Text = pageProviderSettings.Name;

            if (pageProviderSettings.EntryPoint > 0)
            {
                PageData page = this.ContentRepository.Get<PageData>(new PageReference(pageProviderSettings.EntryPoint));

                if (page != null)
                {
                    labelEntryPoint.Text = string.Format(
                        CultureInfo.InvariantCulture, 
                        "<strong>{0}</strong>: {1}<br/>", 
                        "entryPoint",
                        string.Format(CultureInfo.InvariantCulture, "{0} ({1})", page.PageName, page.PageLink.ID));
                }
            }

            if (pageProviderSettings.Root > 0)
            {
                PageData page = this.ContentRepository.Get<PageData>(new PageReference(pageProviderSettings.Root));

                labelRoot.Text = string.Format(
                    CultureInfo.InvariantCulture, 
                    "<strong>{0}</strong>: {1}<br/>", 
                    "root",
                    string.Format(CultureInfo.InvariantCulture, "{0} ({1})", page.PageName, page.PageLink.ID));
            }
            else
            {
                buttonDelete.Visible = false;
                cmsButtonDelete.Visible = false;
            }
        }

        /// <summary>
        ///     Binds the data.
        /// </summary>
        private void BindData()
        {
            this.providerSettings.Clear();

            this.providerSettings = SettingsRepository.Instance.GetAll();

            if (this.providerSettings.Count <= 0)
            {
                ClonedContentProviderSettings contentProviderSettings = new ClonedContentProviderSettings
                                                                      {
                                                                          Name =
                                                                              "No providers found", 
                                                                          Root = 0, 
                                                                          EntryPoint = 0
                                                                      };
                this.providerSettings.Add(contentProviderSettings);
            }

            this.listView.DataSource = this.providerSettings;
            this.listView.DataBind();
        }

        /// <summary>
        /// Shows the error.
        /// </summary>
        /// <param name="errorMessage">
        /// The error message.
        /// </param>
        private void ShowError(string errorMessage)
        {
            this.errorLabel.Text = errorMessage;
            this.errorLabel.Visible = true;
            this.errorContainer.Visible = true;
        }

        #endregion
    }
}