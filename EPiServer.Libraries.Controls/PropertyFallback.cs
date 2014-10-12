using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Framework.Serialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.PropertyControls;
using EPiServer.Web.WebControls;

namespace EPiServer.Libraries.Controls
{
    /// <summary>
    /// WebControl for rendering page properties.
    /// </summary>
    /// <example>
    /// The following example shows how to print the name of the page to HTML:
    /// <code source="../CodeSamples/EPiServerNET/WebControls/PropertySamples.aspx" region="PrintHTML" lang="aspnet" /><note>
    /// The PropertyName attribute is not case sensitive.
    /// </note><para>
    /// For the Property control to be able to read the properties from the page, it needs to
    /// be hosted on a web form or a control that implements the IPageSource interface. The
    /// control will iterate through the control hierarchy looking for this interface, and when
    /// it finds one it will use the CurrentPage property to read the information about the
    /// specific built-in or custom property.
    /// </para><para>
    /// If you put a Property control inside a templated control like the PageList control, that
    /// implements IPageSource, the Property control will use the CurrentPage property of the
    /// template control instead. The PageList then points the Property control to the current
    /// PageData object in its internal PageDataCollection. This is why the two following PageList
    /// examples will print the same:
    /// </para><code source="../CodeSamples/EPiServerNET/WebControls/PropertySamples.aspx" region="PageListSamples" lang="aspnet" />
    /// You can also access the inner PropertyData object through the InnerProperty property, if
    /// you need access to the raw value.
    /// </example>
    /// <remarks>The Property control is used on web forms and user controls and renders the value
    /// of built-in or custom properties to the HTML stream.</remarks>
    [DefaultProperty("PropertyName")]
    [ParseChildren(true)]
    [PersistChildren(false)]
    [ToolboxData("<{0}:PropertyFallback runat=\"server\" />")]
    public sealed class PropertyFallback : WebControl,
                                           INamingContainer,
                                           IPageSource,
                                           IPageControl,
                                           IContentControl,
                                           IContentSource,
                                           IPropertyControlsContainer
    {
        #region Fields

        /// <summary>
        /// The _editor settings
        /// </summary>
        private readonly PropertyEditorSettings _editorSettings = new PropertyEditorSettings();

        /// <summary>
        /// The _overlay settings
        /// </summary>
        private readonly PropertyEditorSettings _overlaySettings = new PropertyEditorSettings();

        /// <summary>
        /// The _render settings
        /// </summary>
        private readonly PropertyRenderSettings _renderSettings = new PropertyRenderSettings();

        /// <summary>
        /// The _content source
        /// </summary>
        private IContentSource _contentSource;

        /// <summary>
        /// The _current context
        /// </summary>
        private PropertyContext _currentContext;

        /// <summary>
        /// The _fallback property data
        /// </summary>
        private PropertyData _fallbackPropertyData;

        /// <summary>
        /// The _is bound
        /// </summary>
        private bool _isBound;

        /// <summary>
        /// The _is dirty
        /// </summary>
        private bool _isDirty;

        /// <summary>
        /// The _page source
        /// </summary>
        private IPageSource _pageSource;

        /// <summary>
        /// The _property data
        /// </summary>
        private PropertyData _propertyData;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EPiServer.Libraries.WebForms.Controls.Property" /> class.
        /// </summary>
        /// <remarks>This control requires that either the <see cref="P:EPiServer.Libraries.WebForms.Controls.Property.PropertyName" />
        /// or the <see cref="P:EPiServer.Libraries.WebForms.Controls.Property.InnerProperty" />
        /// property is set for it to work properly.</remarks>
        public PropertyFallback()
        {
            this.Editable = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:EPiServer.Libraries.WebForms.Controls.Property" /> class with the
        /// given <see cref="T:EPiServer.Core.PropertyData" /> instance.
        /// </summary>
        /// <param name="propertyData">The inner property data object.</param>
        public PropertyFallback(PropertyData propertyData)
            : this()
        {
            this._propertyData = propertyData;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the content source.
        /// </summary>
        /// <value>The content source.</value>
        [Browsable(false)]
        public IContentSource ContentSource
        {
            get
            {
                return this._contentSource ?? (this._contentSource = this.GetContentSource());
            }
            set
            {
                if (this._contentSource == value)
                {
                    return;
                }

                this._propertyData = null;
                this._contentSource = value;
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the Cascading Style Sheet (CSS) class rendered by the Web server control on the client.
        /// </summary>
        /// <value>The CSS class.</value>
        public override string CssClass
        {
            get
            {
                return this.RenderSettings.CssClass ?? string.Empty;
            }
            set
            {
                if (this.CssClass == value)
                {
                    return;
                }

                this.RenderSettings.CssClass = value;
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Gets the currently loaded <see cref="T:EPiServer.Core.IContent">content</see>.
        /// </summary>
        /// <value>Returns information about the currently loaded content, or a content in
        /// a collection when used inside a control.</value>
        /// <remarks><para>
        /// The implementation of <c>CurrentContent</c> is strictly up to the implementing class.
        /// Some of the templated Web controls implement <see cref="T:EPiServer.Core.IContentSource" />, and <c>CurrentContent</c>
        /// will typically refer to the current content being iterated in a collection or an array.
        /// </para>
        /// <para>
        /// Another implementor is the <see cref="T:EPiServer.PageBase" /> class or one of it's subclasses like
        /// <see cref="T:EPiServer.SimplePage" /> or <see cref="T:EPiServer.TemplatePage" />.
        /// <c>CurrentContent</c> in the context of <see cref="T:EPiServer.PageBase" /> refers to the currently
        /// displayed content.
        /// </para>
        /// <para>
        ///   <c>CurrentContent</c> may be null on sources that aren't connected through <c>ContentWebForm</c>.
        /// </para></remarks>
        public IContent CurrentContent
        {
            get
            {
                return this.ContentSource.CurrentContent ?? this.PageSource.CurrentPage;
            }
        }

        /// <summary>
        /// Gets or sets the tag name. If not set a span-tag will be used.
        /// </summary>
        /// <value>The custom tag name.</value>
        /// <example>
        /// Set to "h1" to create a h1-tag around the content.
        /// </example>
        [Category("Appearance")]
        [DefaultValue("")]
        public string CustomTagName
        {
            get
            {
                return this.RenderSettings.CustomTag ?? string.Empty;
            }
            set
            {
                if (this.CustomTagName == value)
                {
                    return;
                }

                this.RenderSettings.CustomTag = value;
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if an error message should be displayed if
        /// no property with the name provided by <see cref="P:EPiServer.Libraries.WebForms.Controls.Property.PropertyName" />
        /// could be found.
        /// </summary>
        /// <value><c>true</c> if [display missing message]; otherwise, <c>false</c>.</value>
        /// <remarks>If the property pointed to by PropertyName does not exist, an error message will be
        /// printed instead. If you want to suppress this error message, set this property to false.
        /// <code><![CDATA[<episerver:property propertyname="MyProperty" runat="server" DisplayMissingMessage='false' />]]></code></remarks>
        [DefaultValue(true)]
        [Bindable(true)]
        [Category("Appearance")]
        public bool DisplayMissingMessage
        {
            get
            {
                return (bool)(this.ViewState["_displayMissingMessage"] ?? true);
            }
            set
            {
                this.ViewState["_displayMissingMessage"] = value;
            }
        }

        /// <summary>
        /// Controls if the property should render it's edit mode.
        /// </summary>
        /// <value><c>true</c> if [edit mode]; otherwise, <c>false</c>.</value>
        /// <remarks>By setting the EditMode property to true the property will be rendered in "edit view",
        /// just like it is when viewed through the Editors View.
        /// <note><b>Note:</b>  Any changes to the edit control will be saved if you enter DOPE mode on
        /// the page and save it, or call the OnDopeSave() client side method from client side script.
        /// </note><code><![CDATA[<episerver:property editmode="True" runat="server" propertyname="MainBody" />
        /// <button onclick="OnDopeSave()">Save</button>]]></code></remarks>
        [Category("Behavior")]
        [DefaultValue(false)]
        public bool EditMode
        {
            get
            {
                return (bool)(this.ViewState["_isEditMode"] ?? false);
            }
            set
            {
                this.ViewState["_isEditMode"] = value;
            }
        }

        /// <summary>
        /// Controls if the property is editable with DOPE
        /// </summary>
        /// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
        /// <remarks>The Property control will give you DOPE (Direct On Page Editing) support if the
        /// underlaying template page supports it (any web form that inherits directly or
        /// indirectly from TemplatePage. If you do not want DOPE support you can either inherit
        /// from SimplePage, or set the Editable property to false, like this:
        /// <code><![CDATA[<episerver:property propertyname="PageName" runat="server" Editable='false' />]]></code></remarks>
        [DefaultValue(true)]
        [Bindable(true)]
        [Category("Behavior")]
        public bool Editable
        {
            get
            {
                return (bool)(this.ViewState["_isEditable"] ?? true) & this.AllowEditable();
            }
            set
            {
                this.ViewState["_isEditable"] = value;
            }
        }

        /// <summary>
        /// Gets or sets any editor settings for the property.
        /// </summary>
        /// <value>The editor settings for the property.</value>
        /// <remarks>These settings can vary from property to property, consult the documentation for each property for more details on
        /// custom properties.</remarks>
        [TypeConverter(typeof(JsonStringToPropertyEditorSettingsConverter))]
        [Category("Appearance")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DefaultValue("")]
        public PropertyEditorSettings EditorSettings
        {
            get
            {
                return this._editorSettings;
            }
        }

        /// <summary>
        /// Gets or sets the name of the fallback property.
        /// </summary>
        /// <value>The name of the fallback property.</value>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public string FallbackPropertyName
        {
            get
            {
                return
                    (string)
                    (this.ViewState["_fallbackPropertyName"]
                     ?? ((this._fallbackPropertyData != null) ? this._fallbackPropertyData.Name : string.Empty));
            }
            set
            {
                if (this.FallbackPropertyName == value)
                {
                    return;
                }

                this._fallbackPropertyData = null;
                this._isDirty = true;
                this.ViewState["_fallbackPropertyName"] = value;
            }
        }

        /// <summary>
        /// Set or get the inner property used by this control
        /// </summary>
        /// <value>The inner property.</value>
        [Browsable(false)]
        public PropertyData InnerProperty
        {
            get
            {
                this.InitializeInnerProperty();

                if (this._propertyData.Value != null)
                {
                    return this._propertyData;
                }

                this.InitializeInnerFallbackProperty();
                return this._fallbackPropertyData;
            }
            set
            {
                this.Editable = false;

                if (value != null)
                {
                    this.PropertyName = value.Name;
                }
                else
                {
                    this._currentContext = null;
                    this._isDirty = true;
                }

                this._propertyData = value;
            }
        }

        /// <summary>
        /// Gets or sets any overlay settings for the property.
        /// </summary>
        /// <value>The overlay settings for the property.</value>
        /// <remarks>These settings can vary from property to property, consult the documentation for each property for more details on
        /// custom properties.</remarks>
        [DefaultValue("")]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [Category("Appearance")]
        [TypeConverter(typeof(JsonStringToPropertyEditorSettingsConverter))]
        public PropertyEditorSettings OverlaySettings
        {
            get
            {
                return this._overlaySettings;
            }
        }

        /// <summary>
        /// The root page to read data from if different from current
        /// </summary>
        /// <value>The page link.</value>
        /// <remarks>If you do not want the Property web control to retrieve the property value from
        /// the currently loaded page (or the current page in a PageDataCollection when inside
        /// a templated web control) you can set the PageLink to point to another page.
        /// <para>
        /// The PageLink property is a PageReference, but you can also assign an int (the ID of the page.)
        /// </para><para><code><![CDATA[<episerver:property runat="server" propertyname="MainBody" PageLink="30" />]]></code></para><para>
        /// or:
        /// </para><para><code><![CDATA[<episerver:property runat="server" propertyname="MainBody" PageLink='<%# EPiServer.Global.EPConfig.StartPage%>' />]]></code></para></remarks>
        [DefaultValue(null)]
        public PageReference PageLink
        {
            get
            {
                return (PageReference)(this.ViewState["_pageLink"] ?? PageReference.EmptyReference);
            }
            set
            {
                if (this.PageLink == value)
                {
                    return;
                }

                this._propertyData = null;
                this._currentContext = null;
                this._isDirty = true;
                this.ViewState["_pageLink"] = value;
            }
        }

        /// <summary>
        /// The property that contains the root page to read data from if different from current
        /// </summary>
        /// <value>The page link property.</value>
        [DefaultValue(null)]
        public string PageLinkProperty
        {
            get
            {
                return (string)(this.ViewState["_pageLinkProperty"] ?? string.Empty);
            }
            set
            {
                if (this.PageLinkProperty == value)
                {
                    return;
                }

                this._propertyData = null;
                this._currentContext = null;
                this._isDirty = false;
                this.ViewState["_pageLinkProperty"] = value;
            }
        }

        /// <summary>
        /// Return the IPageSource implementation that this property control uses to read page data.
        /// </summary>
        /// <value>An IPageSource implementation.</value>
        /// <remarks>The returned instance will usually be the base class for the aspx-page.</remarks>
        [Browsable(false)]
        public IPageSource PageSource
        {
            get
            {
                return this._pageSource ?? (this._pageSource = this.GetPageSource());
            }
            set
            {
                if (this._pageSource == value)
                {
                    return;
                }

                this._propertyData = null;
                this._pageSource = value;
                this._isDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the name of the property that should be displayed by this control.
        /// </summary>
        /// <value>The name of the property.</value>
        /// <example>
        /// The following example shows how to print the name of the page to HTML:
        /// <code><![CDATA[<episerver:property propertyname="PageName" runat="server" />]]></code></example>
        /// <remarks>Please note that the PropertyName attribute is not case sensitive.</remarks>
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        public string PropertyName
        {
            get
            {
                return
                    (string)
                    (this.ViewState["_propertyName"]
                     ?? (this._propertyData != null ? this._propertyData.Name : string.Empty));
            }
            set
            {
                if (this.PropertyName == value)
                {
                    return;
                }

                this._propertyData = null;
                this._isDirty = true;
                this.ViewState["_propertyName"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the scope name separator.
        /// </summary>
        /// <value>The scope name separator.</value>
        /// <remarks>The default value is '.'.</remarks>
        [DefaultValue('.')]
        public char PropertyScopeSeparator
        {
            get
            {
                return (char)(this.ViewState["_scopeNameSeparator"] ?? '.');
            }
            set
            {
                if (this.PropertyScopeSeparator == value)
                {
                    return;
                }

                this.ViewState["_scopeNameSeparator"] = value;
                this._isDirty = true;
            }
        }

        /// <summary>
        /// The value of the loaded property
        /// </summary>
        /// <value>The property value.</value>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Use InnerProperty.Value if you want to retrieve the value of the loaded property.")]
        public object PropertyValue
        {
            get
            {
                return this.InnerProperty != null ? this.InnerProperty.Value : null;
            }
        }

        /// <summary>
        /// Gets or sets any render settings for the property.
        /// </summary>
        /// <value>The render settings for the property.</value>
        /// <remarks>These settings can vary from property to property, consult the documentation for each property for more details on
        /// custom properties.</remarks>
        [PersistenceMode(PersistenceMode.InnerProperty)]
        [DefaultValue("")]
        [Category("Appearance")]
        public PropertyRenderSettings RenderSettings
        {
            get
            {
                return this._renderSettings;
            }
        }

        /// <summary>
        /// Gets or sets the validation group.
        /// </summary>
        /// <value>The validation group.</value>
        [Category("Behavior")]
        [DefaultValue("")]
        public string ValidationGroup
        {
            get
            {
                return (string)(this.ViewState["_validationGroup"] ?? string.Empty);
            }
            set
            {
                this.ViewState["_validationGroup"] = value;
            }
        }

        #endregion

        #region Explicit Interface Properties

        /// <summary>
        /// Gets the current render context of the control instance.
        /// </summary>
        /// <value>The current context.</value>
        PropertyContext IContentControl.CurrentContext
        {
            get
            {
                return this.CurrentContext;
            }
        }

        /// <summary>
        /// Gets the currently loaded <see cref="T:EPiServer.Core.PageData">page</see>.
        /// </summary>
        /// <value>Returns information about the currently loaded page, or a page in
        /// a collection when used inside a control.</value>
        /// <example>
        /// The following code example demonstrates the usage of <c>CurrentPage</c>.
        /// <code>
        /// Response.Write(CurrentPage.PageName);
        /// </code></example>
        /// <remarks><para>
        /// The implementation of <c>CurrentPage</c> is strictly up to the implementing class.
        /// Some of the templated Web controls implement <see cref="T:EPiServer.Core.IPageSource" />, and <c>CurrentPage</c>
        /// will typically refer to the current page being iterated in a collection or an array.
        /// </para>
        /// <para>
        /// Another implementor is the <see cref="T:EPiServer.PageBase" /> class or one of it's subclasses like
        /// <see cref="T:EPiServer.SimplePage" /> or <see cref="T:EPiServer.TemplatePage" />.
        /// <c>CurrentPage</c> in the context of <see cref="T:EPiServer.PageBase" /> refers to the currently
        /// displayed page.
        /// </para>
        /// <para>
        ///   <c>CurrentPage</c> may be null on sources that aren't connected through <c>PageBase</c>.
        /// </para></remarks>
        PageData IPageSource.CurrentPage
        {
            get
            {
                return this.PageSource.CurrentPage;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the ID of the <see cref="T:EPiServer.Core.PageData" /> instance bound to
        /// the current template container.
        /// </summary>
        /// <value>The bound page identifier.</value>
        [Browsable(false)]
        private int BoundPageID
        {
            get
            {
                return (int)(this.ViewState["_boundPageID"] ?? 0);
            }
            set
            {
                this.ViewState["_boundPageID"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:EPiServer.Web.ControlRenderContextBuilder" /> that should be used by the current
        /// control instance.
        /// </summary>
        /// <value>The context builder.</value>
        [Browsable(false)]
        private Injected<ControlRenderContextBuilder> ContextBuilder { get; set; }

        /// <summary>
        /// Gets the current render context of this control instance.
        /// This will not be available until the control has been added to the control tree.
        /// </summary>
        /// <value>The current context.</value>
        private PropertyContext CurrentContext
        {
            get
            {
                return this._currentContext
                       ?? (this._currentContext =
                           this.ContextBuilder.Service.BuildContext(this, this.PropertyName, this.BoundPageID));
            }
        }

        /// <summary>
        /// Gets or sets the last type of the render.
        /// </summary>
        /// <value>The last type of the render.</value>
        private RenderType LastRenderType
        {
            get
            {
                return (RenderType)(this.ViewState["_lastType"] ?? RenderType.Unknown);
            }
            set
            {
                this.ViewState["_lastType"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the object serializer to use when serializing to Json.
        /// </summary>
        /// <value>The object serializer.</value>
        private Injected<IObjectSerializerFactory> ObjectSerializerFactory { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="P:EPiServer.Libraries.WebForms.Controls.Property.PropertyResolver" /> that should be
        /// used by the current control instance.
        /// </summary>
        /// <value>The property resolver.</value>
        [Browsable(false)]
        private Injected<PropertyResolver> PropertyResolver { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Binds a data source to the invoked server control and all its child controls.
        /// </summary>
        /// <exception cref="System.NullReferenceException"></exception>
        public override void DataBind()
        {
            try
            {
                this.OnDataBinding(EventArgs.Empty);
            }
            catch (NullReferenceException ex)
            {
                throw new NullReferenceException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "A error occurred while databinding \"{0}\", some values set on the page where null references [{1}]",
                        this.UniqueID,
                        ex.Message));
            }

            this.SaveBoundState(this.CurrentContext);

            if (this._isDirty || this.LastRenderType != this.GetCurrentRenderType())
            {
                this.Controls.Clear();
                this.ClearChildViewState();
                this.ChildControlsCreated = true;
                this.CreateChildControls();
                this.TrackViewState();
            }

            this.EnsureChildControls();

            for (int index = 0; index < this.Controls.Count; ++index)
            {
                this.Controls[index].DataBind();
            }

            this._isBound = true;
        }

        /// <summary>
        /// Triggers EnsureChildControls for the Property. Mainly used be control developers.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void EnsurePropertyControlsCreated()
        {
            this.EnsureChildControls();
        }

        /// <summary>
        /// Gets the specified content link.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentLink">The content link.</param>
        /// <returns>T.</returns>
        public T Get<T>(ContentReference contentLink) where T : IContentData
        {
            return this.ContentSource.Get<T>(contentLink);
        }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentLink">The content link.</param>
        /// <returns>IEnumerable&lt;T&gt;.</returns>
        public IEnumerable<T> GetChildren<T>(ContentReference contentLink) where T : IContentData
        {
            return this.ContentSource.GetChildren<T>(contentLink);
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// Retrieve a <see cref="T:EPiServer.Core.PageData" /> listing
        /// </summary>
        /// <param name="pageLink">Reference to parent page</param>
        /// <returns>Returns a collection of pages directly below the page referenced by
        /// the <see cref="T:EPiServer.Core.PageReference" /> parameter.</returns>
        /// <example>
        /// The following code example demonstrates the usage of <b>GetChildren</b>.
        /// <code source="../CodeSamples/EPiServer/Core/IPageSourceSamples.cs" region="GetChildren" lang="cs" /></example>
        PageDataCollection IPageSource.GetChildren(PageReference pageLink)
        {
            return this.PageSource.GetChildren(pageLink);
        }

        /// <summary>
        /// Retrieves a <see cref="T:EPiServer.Core.PageData" /> object with information about a page, based on the
        /// <see cref="T:EPiServer.Core.PageReference" /> parameter.
        /// </summary>
        /// <param name="pageLink">Reference to the page being retrieved</param>
        /// <returns>PageData object requested</returns>
        /// <example>
        /// The following code example demonstrates how to get a start page.
        /// <code source="../CodeSamples/EPiServer/Core/IPageSourceSamples.cs" region="GetPage1" lang="cs" />
        /// The following code example demonstrates how to get a page by ID.
        /// <code source="../CodeSamples/EPiServer/Core/IPageSourceSamples.cs" region="GetPage2" lang="cs" /></example>
        PageData IPageSource.GetPage(PageReference pageLink)
        {
            return this.PageSource.GetPage(pageLink);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the content source.
        /// </summary>
        /// <returns>IContentSource.</returns>
        internal IContentSource GetContentSource()
        {
            if (this._contentSource != null)
            {
                return this._contentSource;
            }

            for (Control parent = this.Parent;
                 parent != null && this._contentSource == null;
                 parent = parent.Parent)
            {
                this._contentSource = parent as IContentSource;
            }

            return this._contentSource ?? DataFactory.Instance;
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create
        /// any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            if (this.InnerProperty == null || this.Page == null)
            {
                return;
            }

            RenderType currentRenderType = this.GetCurrentRenderType();

            if (this.Page.IsPostBack && this.LastRenderType != currentRenderType)
            {
                this.ClearChildViewState();
            }

            IPropertyControl propertyControl1 =
                PropertyControlClassFactory.Instance.CreatePropertyControl(
                    this.InnerProperty,
                    new HttpContextWrapper(HttpContext.Current),
                    this.Page,
                    this.RenderSettings.Tag);

            if (propertyControl1 != null)
            {
                this.Controls.Add((Control)propertyControl1);
                propertyControl1.PropertyData = this.InnerProperty;
                propertyControl1.RenderType = currentRenderType;
                propertyControl1.ValidationGroup = this.ValidationGroup;
                propertyControl1.Enabled = this.Enabled;
                PropertyDataControl propertyControl2 = propertyControl1 as PropertyDataControl;

                if (propertyControl2 != null)
                {
                    propertyControl2.AttributeSourceControl = this;
                    propertyControl2.CustomTagName = this.CustomTagName;
                    propertyControl2.CssClass = this.CssClass;
                    this.AssignCustomSettings(propertyControl2);
                }
                propertyControl1.SetupControl();
            }

            this.LastRenderType = currentRenderType;
            this._isDirty = false;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            if (this._isDirty || !PageReference.IsNullOrEmpty(this.PageLink) && !this._isBound)
            {
                this.DataBind();
            }

            this.EnsureChildControls();
            base.OnPreRender(e);
        }

        /// <summary>
        /// Renders the control to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        protected override void Render(HtmlTextWriter writer)
        {
            if (Global.IsDesignTime)
            {
                return;
            }

            if (this.InnerProperty != null)
            {
                this.EnsureChildControls();
                this.RenderChildren(writer);
            }
            else
            {
                this.RenderErrorMessage(writer);
            }
        }

        /// <summary>
        /// Allows editing.
        /// </summary>
        /// <returns><c>true</c> if editing is allowed, <c>false</c> otherwise.</returns>
        private bool AllowEditable()
        {
            PropertyData innerProperty = this.InnerProperty;
            PropertyContext currentContext = this.CurrentContext;

            if (innerProperty == null || innerProperty.IsDynamicProperty || currentContext == null)
            {
                return false;
            }

            ISecurable securable = currentContext.PropertyContainer as ISecurable
                                   ?? currentContext.CurrentContent as ISecurable;
            bool flag = this.CurrentContent != null
                        && this.CurrentContent.ContentLink.CompareToIgnoreWorkID(
                            currentContext.CurrentContent.ContentLink) && currentContext.IsEditable()
                        && !this.IsContainedInTemplate()
                        && (securable == null
                            || securable.GetSecurityDescriptor()
                                   .HasAccess(PrincipalInfo.CurrentPrincipal, AccessLevel.Edit));
            return flag;
        }

        /// <summary>
        /// Assigns the custom settings.
        /// </summary>
        /// <param name="propertyControl">The property control.</param>
        private void AssignCustomSettings(PropertyDataControl propertyControl)
        {
            propertyControl.EditorSettings = this.EditorSettings.Settings;
            propertyControl.RenderSettings = this.RenderSettings.Settings;
            propertyControl.OverlaySettings = this.OverlaySettings.Settings;
        }

        /// <summary>
        /// Gets the type of the current renderer.
        /// </summary>
        /// <returns>RenderType.</returns>
        private RenderType GetCurrentRenderType()
        {
            if (this.EditMode)
            {
                return RenderType.Edit;
            }

            if (!PageEditing.PageIsInEditMode || !this.Editable)
            {
                return RenderType.Default;
            }

            ContentRenderer enclosingContentRenderer = this.GetEnclosingContentRenderer();
            return enclosingContentRenderer != null ? enclosingContentRenderer.RenderType : RenderType.OnPageEdit;
        }

        /// <summary>
        /// Gets the enclosing content renderer.
        /// </summary>
        /// <returns>ContentRenderer.</returns>
        private ContentRenderer GetEnclosingContentRenderer()
        {
            for (Control parent = this.Parent; parent != null; parent = parent.Parent)
            {
                if (parent.GetType() == typeof(ContentRenderer))
                {
                    return parent as ContentRenderer;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the page source.
        /// </summary>
        /// <returns>IPageSource.</returns>
        private IPageSource GetPageSource()
        {
            return this.Page as IPageSource ?? DataFactory.Instance;
        }

        /// <summary>
        /// Initializes the inner fallback property.
        /// </summary>
        private void InitializeInnerFallbackProperty()
        {
            if (this._fallbackPropertyData != null || string.IsNullOrEmpty(this.FallbackPropertyName)
                || this.CurrentContext.PropertyContainer == null)
            {
                return;
            }

            this._fallbackPropertyData =
                this.PropertyResolver.Service.ResolveProperty(
                    this.CurrentContext.PropertyContainer.Property,
                    this.FallbackPropertyName,
                    this.PropertyScopeSeparator);
        }

        /// <summary>
        /// Initializes the inner property.
        /// </summary>
        private void InitializeInnerProperty()
        {
            if (this._propertyData != null || string.IsNullOrEmpty(this.PropertyName)
                || this.CurrentContext.PropertyContainer == null)
            {
                return;
            }

            this._propertyData =
                this.PropertyResolver.Service.ResolveProperty(
                    this.CurrentContext.PropertyContainer.Property,
                    this.PropertyName,
                    this.PropertyScopeSeparator);
        }

        /// <summary>
        /// Checks if this property control is contained somewhere inside a templated control that fetches a
        /// <see cref="T:EPiServer.Core.PageData" />.
        /// </summary>
        /// <returns><c>true</c> if [is contained in template]; otherwise, <c>false</c>.</returns>
        private bool IsContainedInTemplate()
        {
            for (Control control = this; control.Parent != null; control = control.Parent)
            {
                if (control.Parent is PageTemplateContainer
                    || control.Parent is IDataItemContainer && ((IDataItemContainer)control.Parent).DataItem is PageData)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Renders an error message to the specified HTML writer.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter" /> object that receives the control content.</param>
        /// <remarks>This method is responsible for figuring out and render an error message.
        /// It will look at the <see cref="P:EPiServer.Libraries.WebForms.Controls.Property.DisplayMissingMessage" /> to decide
        /// if it should render certain errors.</remarks>
        private void RenderErrorMessage(HtmlTextWriter writer)
        {
            if (string.IsNullOrEmpty(this.PropertyName.Trim()))
            {
                writer.WriteLine("[Error: The PropertyName is not set.]");
            }
            else if (this.CurrentContext.PropertyContainer == null)
            {
                if (!this.DisplayMissingMessage)
                {
                    return;
                }

                writer.WriteLine(
                    "[Error: Property control for property '{0}' is contained in a page/control/template that does not contain any properties.]",
                    this.PropertyName);
            }
            else if (this.PropertyResolver.Service.ResolveProperty(
                this.CurrentContext.PropertyContainer.Property,
                this.PropertyName,
                this.PropertyScopeSeparator) == null)
            {
                if (!this.DisplayMissingMessage)
                {
                    return;
                }

                writer.WriteLine("[Error: Unable to find the property named '{0}'.]", this.PropertyName);
            }
            else
            {
                writer.WriteLine("[Error: Unknown error with property named '{0}'.]", this.PropertyName);
            }
        }

        /// <summary>
        /// Saves the bound state.
        /// </summary>
        /// <param name="context">The context.</param>
        private void SaveBoundState(PropertyContext context)
        {
            if (context == null || context.PropertyContainer == null
                || (context.PropertyContainer != context.CurrentContent
                    || !string.IsNullOrEmpty(context.CurrentContent.ContentLink.ProviderName)))
            {
                return;
            }

            this.BoundPageID = context.CurrentContent.ContentLink.ID;
        }

        #endregion
    }
}