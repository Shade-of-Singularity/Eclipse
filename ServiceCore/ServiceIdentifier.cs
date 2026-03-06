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
    /// Do not use this class unless you know what you are doing! (or unless you are writing your own CRTP ServiceCore extension)
    /// Marks class/instance (e.g. <see cref="Service{T}"/> or <see cref="IService{T}"/>) as an identifier in inheritance tree.
    /// </summary>
    /// <remarks>
    /// Interfaces are prioritized over classes (e.g. <see cref="IService{T}"/> prioritized over <see cref="Service{T}"/>)
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceIdentifierAttribute : Attribute { }
}
