
using ServiceCore.Reflection;

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
    public static partial class Services
    {
        /// <summary>
        /// Entry with runtime information about a service instance.
        /// </summary>
        /// <param name="Service">Instance of a service.</param>
        /// <param name="Descriptor">Descriptor of said service.</param>
        public readonly record struct ActiveService(IService Service, ServiceDescriptor Descriptor) { }
    }
}
