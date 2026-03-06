/// - - -    Copyright (c) 2025     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///         http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// ]]>

using ServiceCore.Loading;

namespace ServiceCore
{
    /// <summary>
    /// Default implementation for <see cref="ITerminationArgs"/>.
    /// </summary>
    public sealed class DefaultTerminationArgs() : ITerminationArgs
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
        public DefaultTerminationArgs(EngineState? state) : this() => Setup(state);

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
