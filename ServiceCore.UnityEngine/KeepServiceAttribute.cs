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
    /// When attached to <see cref="MonoService{T}"/> implementation - tells it how to treat new instances of a service.
    /// </summary>
    /// <remarks>
    /// When not specified, implicitly implements <see cref="KeepInstance.Older"/>
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class KeepServiceAttribute(KeepInstance mode) : Attribute
    {
        /// <summary>
        /// Tells which instance to keep for an service, when a new one is introduced.
        /// </summary>
        public readonly KeepInstance Mode = mode;
    }
}
