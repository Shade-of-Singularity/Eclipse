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

using System;

namespace ServiceCore
{
    /// <summary>
    /// Runs methods with this attribute when <see cref="ServiceCore"/>.<see cref="Engine"/> terminates.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceCoreTerminationAttribute(TerminationTiming timing) : Attribute
    {
        /// <summary>
        /// When attribute should be employed.
        /// </summary>
        public readonly TerminationTiming Timing = timing;

        /// <summary>
        /// Default constructor. Runs method in the latest possible point during engine unloading.
        /// <para>See also: <see cref="TerminationTiming.AfterEngineTermination"/>.</para>
        /// </summary>
        public ServiceCoreTerminationAttribute() : this(TerminationTiming.AfterEngineTermination) { }
    }
}
