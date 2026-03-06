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
    /// Describes a state of the <see cref="Engine"/> during or after initialization.
    /// It also stores some info about currently loaded-in mods and stuff like that.
    /// You can use it to retrieve the info about incompatible modifications, why engine failed, or if it works properly at the moment.
    /// </summary>
    /// <param name="status">Initial status of this <see cref="EngineState"/> instance.</param>
    public sealed class EngineState(EngineStatus status) : IInitializationArgs, ITerminationArgs, ICommonStartupArgs
    {
        /// <inheritdoc/>
        public EngineStatus Status { get; internal set; } = status;

        /// <inheritdoc/>
        public bool IsDependenciesBroken { get; internal set; }

        /// <inheritdoc/>
        public DependencyMap Modifications { get; internal set; } = [];

        /// <inheritdoc/>
        public void Setup(EngineState? state)
        {
            if (state is null)
            {
                Status = EngineStatus.Terminated;
                IsDependenciesBroken = false;
                Modifications = [];
                return;
            }

            Status = state.Status;
            IsDependenciesBroken = state.IsDependenciesBroken;
            Modifications = state.Modifications;
        }
    }
}