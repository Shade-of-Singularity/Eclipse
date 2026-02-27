using ServiceCore.Modding;

namespace ServiceCore
{
    /// <summary>
    /// Describes arguments, common between <see cref="IInitializationArgs"/> and <see cref="ITerminationArgs"/>.
    /// </summary>
    /// <remarks>
    /// Implemented by <see cref="EngineState"/>, allowing you to use it directly in the initialization/termination.
    /// </remarks>
    /// <seealso cref="IService.InvokeInitialize(IInitializationArgs)"/>
    /// <seealso cref="IService.InvokeTerminate(ITerminationArgs)"/>
    public interface ICommonStartupArgs
    {
        /// <summary>
        /// (When provided by the <see cref="Engine"/>) Current status of the <see cref="Engine"/>.
        /// </summary>
        public EngineStatus Status { get; }

        /// <summary>
        /// (When provided by the <see cref="Engine"/>) Whether dependencies were not resolved properly.
        /// (As a core dev) You can use this value to only partially initialize your services, to show a warning on a screen.
        /// Introduced to avoid fully loading LocalizationServices (and similar) when dependencies are broken, and an restart will be needed anyway.
        /// </summary>
        /// <remarks>
        /// Even with broken dependencies, <see cref="Status"/> will be set to <see cref="EngineStatus.Initialized"/>!
        /// <see cref="EngineStatus.InitializationBroken"/> and <see cref="EngineStatus.TerminationBroken"/>
        /// is shown ONLY when <see cref="Engine"/> breaks during <see cref="Engine.Initialize(InitializationContext)"/> or <see cref="Engine.Terminate"/>
        /// </remarks>
        public bool IsDependenciesBroken { get; }

        /// <summary>
        /// All loaded modifications (a.k.a. <see cref="Loading.ILoadingSource"/>s).
        /// Also describes dependencies between all of them.
        /// </summary>
        public DependencyMap? Modifications { get; }

        /// <summary>
        /// Supplies values from <see cref="EngineState"/>.
        /// This method is called by <see cref="Engine"/> during initialization.
        /// </summary>
        /// <param name="state">Engine state to setup.</param>
        public void Setup(EngineState? state);
    }
}
