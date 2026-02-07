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
    /// Current status of the engine.
    /// </summary>
    public enum EngineStatus : byte
    {
        /// <summary>
        /// (Default) Engine is terminated and waits for <see cref="Engine.Initialize"/> to be used.
        /// </summary>
        /// <remarks>
        /// If automatic initialization enabled in <see cref="EclipseConfiguration"/>
        /// by setting <see cref="EclipseConfiguration.InitializationType"/>
        /// to anything but <see cref="AutomaticStartupType.Manual"/> - will be in terminated state very briefly.
        /// </remarks>
        Terminated = 0b0000_0000,

        /// <summary>
        /// Indicates that <see cref="Engine"/> is in active initialization right now.
        /// Will be set to <see cref="Initialized"/> once initialization is finished.
        /// </summary>
        Initializing = 0b0000_0001,

        /// <summary>
        /// Indicates that <see cref="Engine"/> is fully initialized.
        /// </summary>
        Initialized = 0b0000_0010,

        /// <summary>
        /// Indicates that <see cref="Engine"/> is being terminated.
        /// Will be set to <see cref="Terminated"/> once terminated is finished.
        /// </summary>
        Terminating = 0b0000_0100,

        /// <summary>
        /// Represents that <see cref="Engine"/> got irreversibly broken during <see cref="Initializing"/>
        /// and cannot be restored without full application reloading.
        /// </summary>
        /// <remarks>
        /// After thorough engine testing, should only happen if mods or your custom code mess-up the system soo much,
        /// that we just can't do anything about it.
        /// </remarks>
        InitializationBroken = 0b0100_0000,

        /// <summary>
        /// Represents that <see cref="Engine"/> got irreversibly broken during <see cref="Terminating"/>
        /// and cannot be restored without full application reloading.
        /// </summary>
        /// <remarks>
        /// After thorough engine testing, should only happen if mods or your custom code mess-up the system soo much,
        /// that we just can't do anything about it.
        /// </remarks>
        TerminationBroken = 0b1000_0000,
    }
}
