// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputSettings.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output
{
    using System;
    using System.Web.UI.WebControls;

    using EPiServer.PlugIn;

    using log4net;

    /// <summary>
    ///     Setttings for the translations
    /// </summary>
    [GuiPlugIn(Area = PlugInArea.None, DisplayName = "Output Settings")]
    public class OutputSettings
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="OutputSettings" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OutputSettings));

        /// <summary>
        ///     The instance.
        /// </summary>
        private static OutputSettings instance;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="OutputSettings" /> class.
        /// </summary>
        public OutputSettings()
        {
            PlugInSettings.SettingsChanged += PlugInSettingsChanged;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets the instance.
        /// </summary>
        public static OutputSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OutputSettings();
                }

                try
                {
                    PlugInSettings.AutoPopulate(instance);
                }
                catch (Exception exception)
                {
                    Logger.Error("[OutputFormats] Error loading translation settings.", exception);
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

        /// <summary>
        ///     Gets or sets the location of the print stylesheet .
        /// </summary>
        [PlugInProperty(Description = "Print stylesheet location.", AdminControl = typeof(TextBox), 
            AdminControlValue = "Text")]
        public virtual string StyleSheet { get; set; }

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