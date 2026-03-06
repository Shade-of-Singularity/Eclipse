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
