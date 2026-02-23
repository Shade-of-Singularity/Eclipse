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
using System.Collections.Generic;

namespace ServiceCore
{
    public static partial class Services
    {
        /// <summary>
        /// Entry with information about a service.
        /// </summary>
        public readonly partial struct ServiceEntry(IService service, Type[] associations)
        {
            /// <summary>
            /// Service instance that can be referenced/used.
            /// </summary>
            public readonly IService service = service;
            /// <summary>
            /// All type keys associated with given <see cref="service"/>.
            /// </summary>
            public readonly Type[] associations = associations;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Static Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static readonly List<Type> m_Associations = new(12);




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Static Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public static ServiceEntry Construct(IService service)
            {
                var associations = m_Associations;
                lock (associations)
                {
                    associations.Clear();

                    Type type = service.GetType();
                    if (type.IsDefined(typeof(IgnoreServiceBranchAttribute), inherit: true))
                    {
                        return new(service, [type]); // Temp solution.
                    }

                    type.FindInterfaces(Filter, null); // Registers all interfaces implementing this service.
                    do
                    {
                        // Registers all classes on the way to the base.
                        associations.Add(type);
                        type = type.BaseType;
                    }
                    while (type is not null && type != typeof(object));
                    return new ServiceEntry(service, [.. associations]);

                    // Simplifications:
                    bool Filter(Type type, object? arg)
                    {
                        if (typeof(IService).IsAssignableFrom(type) && !type.IsDefined(typeof(IgnoreServiceAttribute), inherit: false))
                        {
                            associations.Add(type);
                        }

                        // Always return false, to not form an internal array.
                        // TODO: Check source code to see how much resources, if any, this thing eats on idle run.
                        return false;
                    }
                }
            }
        }
    }
}
