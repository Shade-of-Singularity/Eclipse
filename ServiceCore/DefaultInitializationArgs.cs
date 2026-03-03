using ServiceCore.Modding;

namespace ServiceCore
{
    /// <summary>
    /// Default implementation for <see cref="IInitializationArgs"/>.
    /// </summary>
    public sealed class DefaultInitializationArgs() : IInitializationArgs
    {
        /// <inheritdoc/>
        public EngineStatus Status { get; internal set; } = EngineStatus.Terminated;

        /// <inheritdoc/>
        public bool IsDependenciesBroken { get; internal set; } = false;

        /// <inheritdoc/>
        public DependencyMap Modifications { get; internal set; } = DependencyMap.Native;

        /// <summary>
        /// Automatically setups all parameters from provided <see cref="EngineState"/> using <see cref="Engine.State"/>
        /// </summary>
        public DefaultInitializationArgs(EngineState? state) : this() => Setup(state);

        /// <inheritdoc/>
        public void Setup(EngineState? state)
        {
            if (state is null)
            {
                Status = EngineStatus.Terminated;
                IsDependenciesBroken = false;
                Modifications = DependencyMap.Native;
                return;
            }

            Status = state.Status;
            IsDependenciesBroken = state.IsDependenciesBroken;
            Modifications = state.Modifications;
        }
    }
}
