// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OutputInitialization.cs" company="Jeroen Stemerdink">
//   Copyright© 2013 Jeroen Stemerdink. All Rights Reserved.
// </copyright>
// <summary>
//   The route initialization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EPiServer.Libraries.Output
{
    using System.Web.Routing;

    using EPiServer.Framework;
    using EPiServer.Framework.Initialization;
    using EPiServer.Web.Routing;

    using log4net;

    using InitializationModule = EPiServer.Web.InitializationModule;

    /// <summary>
    ///     The route initialization.
    /// </summary>
    [ModuleDependency(typeof(InitializationModule))]
    public class OutputInitialization : IInitializableModule
    {
        #region Fields

        /// <summary>
        ///     Initializes the <see cref="LogManager">LogManager</see> for the <see cref="OutputInitialization" /> class.
        /// </summary>
        private static readonly ILog Logger = LogManager.GetLogger(typeof(OutputInitialization));

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

            Logger.Info("[OutputFormats] Initializing output formats.");

            OutputPartialRouter partialRouter = new OutputPartialRouter();
            RouteTable.Routes.RegisterPartialRouter(partialRouter);

            this.initialized = true;

            Logger.Info("[OutputFormats] Output formats initialized.");
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