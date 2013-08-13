// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChannelSettings.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output.Channels
{
    using System;
    using System.Web.UI.WebControls;

    using EPiServer.PlugIn;

    using log4net;

    /// <summary>
    ///     Setttings for the translations
    /// </summary>
    [GuiPlugIn(Area = PlugInArea.None, DisplayName = "Output Channel Settings")]
    public class ChannelSettings
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="ChannelSettings" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ChannelSettings));

        /// <summary>
        ///     The instance.
        /// </summary>
        private static ChannelSettings instance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChannelSettings" /> class.
        /// </summary>
        public ChannelSettings()
        {
            PlugInSettings.SettingsChanged += PlugInSettingsChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        public static ChannelSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ChannelSettings();
                }

                try
                {
                    PlugInSettings.AutoPopulate(instance);
                }
                catch (Exception exception)
                {
                    Logger.Error("[OutputFormats] Error loading output channel settings.", exception);
                }

                return instance;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether [to enable JSON output].
        /// </summary>
        [PlugInProperty(Description = "Enable JSON output.", AdminControl = typeof(CheckBox), 
            AdminControlValue = "Checked")]
        public virtual bool EnableJSON { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [to enable JSON output].
        /// </summary>
        [PlugInProperty(Description = "Enable PDF output.", AdminControl = typeof(CheckBox), 
            AdminControlValue = "Checked")]
        public virtual bool EnablePDF { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [to enable TXT output].
        /// </summary>
        [PlugInProperty(Description = "Enable TXT output.", AdminControl = typeof(CheckBox), 
            AdminControlValue = "Checked")]
        public virtual bool EnableTXT { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [to enable XML output].
        /// </summary>
        [PlugInProperty(Description = "Enable XML output.", AdminControl = typeof(CheckBox), 
            AdminControlValue = "Checked")]
        public virtual bool EnableXML { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// The plug in settings changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private static void PlugInSettingsChanged(object sender, EventArgs e)
        {
            instance = null;
        }

        #endregion
    }
}