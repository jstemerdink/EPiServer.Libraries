// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputFormatsInitialization.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Mvc.Output.Channels
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.WebPages;

    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Libraries.Output;
    using EPiServer.ServiceLocation;
    using EPiServer.Web;

    using InitializationModule = EPiServer.Web.InitializationModule;

    /// <summary>
    ///     Adds new display modes for different output formats
    /// </summary>
    [ModuleDependency(typeof(InitializationModule))]
    public class OutputFormatsInitialization : IInitializableModule
    {
        #region Static Fields

        /// <summary>
        ///     The display channel service.
        /// </summary>
        private static readonly DisplayChannelService CurrentDisplayChannelService =
            ServiceLocator.Current.GetInstance<DisplayChannelService>();

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
            if (context == null || context.HostType != HostType.WebApplication)
            {
                return;
            }

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.JSON) { ContextCondition = IsJsonDisplayModeActive }, 
                    0);

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.XML) { ContextCondition = IsXmlDisplayModeActive }, 
                    0);

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.Text) { ContextCondition = IsTxtDisplayModeActive }, 
                    0);

            context.Locate.DisplayChannelService()
                .RegisterDisplayMode(
                    new DefaultDisplayMode(OutputConstants.PDF) { ContextCondition = IsPdfDisplayModeActive }, 
                    0);
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
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether [is json display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is json display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsJsonDisplayModeActive(HttpContextBase httpContext)
        {
            return
                CurrentDisplayChannelService.GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.JSON, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is PDF display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is PDF display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsPdfDisplayModeActive(HttpContextBase httpContext)
        {
            return
                CurrentDisplayChannelService.GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.PDF, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is TXT display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is TXT display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsTxtDisplayModeActive(HttpContextBase httpContext)
        {
            return
                CurrentDisplayChannelService.GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.Text, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines whether [is XML display mode active] [the specified HTTP context].
        /// </summary>
        /// <param name="httpContext">
        /// The HTTP context.
        /// </param>
        /// <returns>
        /// <c>true</c> if [is XML display mode active] [the specified HTTP context]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsXmlDisplayModeActive(HttpContextBase httpContext)
        {
            return
                CurrentDisplayChannelService.GetActiveChannels(httpContext)
                    .Any(c => string.Equals(c.ChannelName, OutputConstants.XML, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}