using ServiceCore.Modding;

namespace ServiceCore
{
    /// <summary>
    /// Default implementation for <see cref="IInitializationArgs"/>.
    /// </summary>
    /// <param name="state">Engine state to apply.</param>
    public struct DefaultInitializationArgs(EngineState state) : IInitializationArgs
    {
        /// <inheritdoc/>
        public EngineStatus Status { get; internal set; } = state.Status;

        /// <inheritdoc/>
        public bool IsDependenciesBroken { get; internal set; } = state.IsDependenciesBroken;

        /// <inheritdoc/>
        public DependencyMap Modifications { get; internal set; } = state.Modifications;

        /// <inheritdoc/>
        public void Setup(EngineState state)
        {
            Status = state.Status;
            IsDependenciesBroken = state.IsDependenciesBroken;
            Modifications = state.Modifications;
        }
    }
}
