// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiInitialization.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.WebApi.Output
{
    using System.Web.Http;

    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;

    using log4net;

    using InitializationModule = EPiServer.Web.InitializationModule;

    /// <summary>
    ///     The route initialization.
    /// </summary>
    [ModuleDependency(typeof(InitializationModule))]
    public class WebApiInitialization : IInitializableModule
    {
        #region Static Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="WebApiInitialization" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WebApiInitialization));

        #endregion

        #region Fields

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

            Logger.Info("[OutputFormats] Initializing output WebApi.");

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            this.initialized = true;

            Logger.Info("[OutputFormats] Output WebApi initialized.");
        }

        /// <summary>
        /// The preload.
        /// </summary>
        /// <param name="parameters">
        /// The parameters.
        /// </param>
        public void Preload(string[] parameters)
        {
        }

        /// <summary>
        /// The uninitialize.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public void Uninitialize(InitializationEngine context)
        {
            this.initialized = false;
        }

        #endregion
    }
}