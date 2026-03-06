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
    /// Base class for <see cref="BeforeServiceInitializedAttribute"/> and <see cref="AfterServiceInitializedAttribute"/> for faster (?) reflection.
    /// </summary>
    public abstract class ServiceInitializationAttribute(Type service) : Attribute
    {
        /// <summary>
        /// <see cref="AttributeTargets"/> any of <see cref="ServiceInitializationAttribute"/>s should support.
        /// </summary>
        public const AttributeTargets Targets = AttributeTargets.Method;
        /// <summary>
        /// Whether any <see cref="ServiceInitializationAttribute"/>s should be inheritable.
        /// </summary>
        public const bool Inheritable = false;
        /// <summary>
        /// Whether any <see cref="ServiceInitializationAttribute"/>s should allow multiple declarations.
        /// </summary>
        public const bool AllowsMultiple = true;

        /// <summary>
        /// Order in which preload methods should be invoked.
        /// </summary>
        /// <remarks>
        /// <see cref="InvokeOrder"/> still applies even when <see cref="Service"/> runs on a background thread
        /// - it will run within synchronization context of <see cref="Service"/>.
        /// </remarks>
        public int InvokeOrder { get; set; }

        /// <summary>
        /// Whether method can be executed in a background thread.
        /// </summary>
        /// <remarks>
        /// It won't always run in background when set to <c>true</c>, but whenever possible - will run in parallel.
        /// </remarks>
        public bool ThreadSafe { get; set; }

        /// <summary>
        /// Reference service to use. Method with this attribute will be executed after this service is initialized.
        /// </summary>
        public Type Service { get; } = service;
    }
}
