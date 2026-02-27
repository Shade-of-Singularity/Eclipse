using ServiceCore.Modding;

namespace ServiceCore
{
    /// <summary>
    /// Default implementation for <see cref="IInitializationArgs"/>.
    /// </summary>
    public sealed class DefaultInitializationArgs() : IInitializationArgs
    {
        /// <inheritdoc/>
        public EngineStatus Status { get; internal set; }

        /// <inheritdoc/>
        public bool IsDependenciesBroken { get; internal set; }

        /// <inheritdoc/>
        public DependencyMap? Modifications { get; internal set; }

        /// <inheritdoc/>
        public void Setup(EngineState? state)
        {
            if (state is null)
            {
                Status = EngineStatus.Terminated;
                IsDependenciesBroken = false;
                Modifications = null;
                return;
            }

            Status = state.Status;
            IsDependenciesBroken = state.IsDependenciesBroken;
            Modifications = state.Modifications;
        }
    }
}
