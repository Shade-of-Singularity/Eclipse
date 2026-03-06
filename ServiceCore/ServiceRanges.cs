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
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Generic implementation for <see cref="ServiceRanges"/> to quickly access <see cref="ServiceRange"/> struct.
    /// </summary>
    /// <typeparam name="T">Class implementing <see cref="IService"/>.</typeparam>
    public static class ServiceRanges<T> where T : IService
    {
        /// <summary>
        /// <see cref="ServiceRange"/> for given service under serviceType T.
        /// </summary>
        public static readonly ServiceRange Range = ServiceRanges.Retrieve(typeof(T));
    }

    /// <summary>
    /// Stores all <see cref="ServiceRange"/> instances for all <see cref="IService"/> types that require it.
    /// </summary>
    public static class ServiceRanges
    {
        static readonly ConcurrentDictionary<Type, ServiceRange> m_Ranges = [];

        /// <summary>
        /// Retrieves <see cref="ServiceRange"/> from an internal cache, or creates new one.
        /// </summary>
        /// <param name="serviceType">Type, which has at least one <see cref="ServiceIdentifierAttribute"/> in implementation tree.</param>
        /// <returns><see cref="ServiceRange"/> descripting <paramref name="serviceType"/>.</returns>
        public static ServiceRange Retrieve(Type serviceType)
        {
            if (serviceType is null) throw new ArgumentNullException(nameof(serviceType));
            return m_Ranges.GetOrAdd(serviceType, static (type) =>
            {
                if (!typeof(IService).IsAssignableFrom(type))
                {
                    return ServiceRange.Invalid;
                }

                // TODO (Optimization): Avoid array allocation for services with one identifier using `Type? first = null;` here.
                //  Or better yet - use Span with int indexes for interfaces, and approach from above for classes.
                Type[] identifiers = [];

                // Checks classes first, to make sure that the top-most ServiceIdentifier class is first in the list.
                // Needed to support property ServiceRange.First.
                Type temp = type;

                // Also runs static .ctor to construct all ServiceDescriptors.
                // TODO: Don't run constructors is Engine was ever initialized (if it will naturally run static .ctor on all services).
                // TODO: Provide a Roslyn generator for ServiceIdentifiers instead for AOT compliance.
                do
                {
                    if (temp.IsDefined(typeof(ServiceIdentifierAttribute), inherit: false))
                    {
                        int length = identifiers is null ? 0 : identifiers.Length;
                        Array.Resize(ref identifiers, length + 1);
                        identifiers[length] = temp;
                    }

                    RuntimeHelpers.RunClassConstructor(temp.TypeHandle);
                    temp = temp.BaseType;
                }
                while (temp is not null);

                var interfaces = type.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    temp = interfaces[i];
                    if (temp.IsDefined(typeof(ServiceIdentifierAttribute), inherit: false))
                    {
                        int length = identifiers is null ? 0 : identifiers.Length;
                        Array.Resize(ref identifiers, length + 1);
                        identifiers[length] = temp;
                    }

                    RuntimeHelpers.RunClassConstructor(temp.TypeHandle);
                }

                // Initializes the range.
                if (identifiers.Length == 0) return ServiceRange.Invalid;
                ServiceDescriptor[] descriptors = new ServiceDescriptor[identifiers.Length];
                for (int i = 0; i < identifiers.Length; i++)
                {
                    descriptors[i] = ServiceDescriptor.Get(identifiers[i]);
                }

                return new(descriptors);
            });
        }
    }
}
