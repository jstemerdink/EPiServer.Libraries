// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputFormatsInitialization.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Mvc.Output.Channels
{
    using System.Web.WebPages;

    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Libraries.Output;
    using EPiServer.Libraries.Output.Channels;
    using EPiServer.Web;

    using log4net;

    using InitializationModule = EPiServer.Web.InitializationModule;

    /// <summary>
    ///     Adds new display modes for different output formats
    /// </summary>
    [ModuleDependency(typeof(InitializationModule))]
    public class OutputFormatsInitialization : IInitializableModule
    {
        #region Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="OutputFormatsInitialization" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OutputFormatsInitialization));

        /// <summary>
        ///     The initialized.
        /// </summary>
        private bool initialized;

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Initialize(InitializationEngine context)
        {
            if (this.initialized || context == null || context.HostType != HostType.WebApplication)
            {
                return;
            }

            Logger.Info("[OutputFormats] Initializing output channels.");

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.JSON) { ContextCondition = ChannelHelper.IsJsonDisplayModeActive }, 
                    0);

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.XML) { ContextCondition = ChannelHelper.IsXmlDisplayModeActive }, 
                    0);

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.Text) { ContextCondition = ChannelHelper.IsTxtDisplayModeActive }, 
                    0);

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.PDF) { ContextCondition = ChannelHelper.IsPdfDisplayModeActive }, 
                    0);

            this.initialized = true;

            Logger.Info("[OutputFormats] Output channels initialized.");
        }

        /// <summary>
        /// Preloads the module.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        /// <remarks>
        /// This method is only available to be compatible with "AlwaysRunning" applications in .NET 4 / IIS 7.
        ///     It currently serves no purpose.
        /// </remarks>
        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        /// Resets the module into an uninitialized state.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <remarks>
        /// <para>
        /// This method is usually not called when running under a web application since the web app may be shut down very
        ///         abruptly, but your module should still implement it properly since it will make integration and unit testing
        ///         much simpler.
        ///     </para>
        /// <para>
        /// Any work done by
        ///         <see cref="M:EPiServer.Framework.IInitializableModule.Initialize(EPiServer.Framework.Initialization.InitializationEngine)"/>
        ///         as well as any code executing on
        ///         <see cref="E:EPiServer.Framework.Initialization.InitializationEngine.InitComplete"/> should be reversed.
        ///     </para>
        /// </remarks>
        public void Uninitialize(InitializationEngine context)
        {
            this.initialized = false;
        }

        #endregion
    }
}