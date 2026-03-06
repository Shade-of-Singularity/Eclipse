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

//using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Useful or Quality-of-Life extensions for <see cref="IService"/>.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// (Cached via CRTP) Retrieves <see cref="ServiceDescriptor"/> from <see cref="IService{T}.Descriptor"/> field directly.
        /// </summary>
        /// <param name="service">Service to retrieve a <see cref="ServiceDescriptor"/> for.</param>
        /// <returns><see cref="ServiceDescriptor"/> describing provided <paramref name="service"/>.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ServiceDescriptor GetDescriptor(this IService service) => service.Descriptor;
    }
}
