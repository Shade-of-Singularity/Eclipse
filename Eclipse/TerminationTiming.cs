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

namespace Eclipse
{
    /// <summary>
    /// Indicates when <see cref="EclipseTerminationAttribute"/> should run underlying method.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// Also, uses <see cref="ushort"/> instead of <see cref="byte"/> in case we will need 10-12 callbacks.
    /// TODO: Make callback system in <see cref="Engine"/> which allows you to subscribe to each termination timing during Engine termination.
    public enum TerminationTiming : ushort
    {
        /// <summary>
        /// Termination method is never run.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Runs before anything was terminated from the engine.
        /// </summary>
        BeforeEngineTermination = 0b0000_0000_0000_0001,

        /// <summary>
        /// Runs after entire engine was terminated.
        /// </summary>
        AfterEngineTermination = 0b1000_0000_0000_0000,
    }
}
