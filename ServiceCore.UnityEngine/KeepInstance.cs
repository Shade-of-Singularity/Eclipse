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

namespace ServiceCore
{
    /// <summary>
    /// Tells which instance to keep for an service, when a new one is introduced.
    /// Uses <see cref="Older"/> by default.
    /// </summary>
    public enum KeepInstance : byte
    {
        /// <summary>
        /// (Default) Keeps older service reference.
        /// </summary>
        Older = 0,

        /// <summary>
        /// Keeps newer service reference.
        /// </summary>
        /// <remarks>
        /// Consider using <see cref="MonoManager{T}"/> instead.
        /// </remarks>
        Newer = 1,
    }
}
