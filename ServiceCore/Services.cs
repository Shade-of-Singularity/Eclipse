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
using System.Diagnostics.CodeAnalysis;

namespace ServiceCore
{
    /// <summary>
    /// Stores references to all the services, to not overload <see cref="Engine"/>.
    /// </summary>
    public static partial class Services
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Prefix for log messages sent from this class.
        /// </summary>
        public const string LogPrefix = "[" + nameof(Services) + "]";




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Invoked when services was instantiated, but haven't initialized yet.
        /// </summary>
        public static event Action? OnServicesInitializing;
        /// <summary>
        /// Invoked when services was fully initialized.
        /// </summary>
        public static event Action? OnServicesInitialized;
        /// <summary>
        /// Invoked when services are about to be terminated. Fires after 'Engine.OnEngineTerminating' (TODO: Add reference).
        /// </summary>
        public static event Action? OnServicesTerminating;
        /// <summary>
        /// Invoked when services was fully terminated.
        /// </summary>
        public static event Action? OnServicesTerminated;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>        
        /// <summary>
        /// <see cref="IReadOnlyDictionary{TKey, TValue}"/> containing all currently present <see cref="ActiveService"/>s.
        /// </summary>
        public static IReadOnlyDictionary<Type, ActiveService> Map => m_Services;
        
        /// <summary>
        /// Enumerator over all <see cref="ActiveService"/> of all registered services.
        /// </summary>
        public static IEnumerable<ActiveService> Entries
        {
            get
            {
                lock (m_Services)
                {
                    // TODO: Avoid duplicates, which appear due to map mapping services to multiple keys for optimization.
                    foreach (var entry in m_Services.Values)
                    {
                        yield return entry;
                    }
                }
            }
        }

        /// <summary>
        /// Enumerator over all registered services.
        /// </summary>
        public static IEnumerable<IService> List
        {
            get
            {
                lock (m_Services)
                {
                    // TODO: Avoid duplicates, which appear due to map mapping services to multiple keys for optimization.
                    foreach (var entry in m_Services.Values)
                    {
                        yield return entry.Service;
                    }
                }
            }
        }





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, ActiveService> m_Services = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Checks if there is service with requested type <typeparamref Identifier="T"/>.
        /// </summary>
        /// <remarks>
        /// Less optimized than strongly-typed <see cref="IService{TService}.Exist"/>.
        /// </remarks>
        public static bool Has<T>() where T : class, IService
        {
            // Note: Do we need a type check at all? Won't it break the logic in common use cases?
            return m_Services.TryGetValue(typeof(T), out ActiveService entry) && entry.Service is T;
        }

        /// <summary>
        /// Checks if there is a service with requested <paramref Identifier="type"/>.
        /// </summary>
        /// <remarks>
        /// Less optimized than strongly-typed <see cref="IService{TService}.Exist"/>.
        /// </remarks>
        public static bool Has(Type type)
        {
            // Note: Do we need a type check at all? Won't it break the logic in common use cases?
            return m_Services.TryGetValue(type, out ActiveService entry) && entry.Service.GetType() == type;
        }




        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{TService}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Never throws. Instead, returns <c>null</c> if service is not defined or its type was changed.
        /// </remarks>
        public static T? Get<T>() where T : class, IService
        {
            if (m_Services.TryGetValue(typeof(T), out ActiveService entry))
            {
                return entry.Service as T;
            }

            return default;
        }

        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{TService}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Never throws. Instead, returns <c>null</c> if service is not defined or its type was changed.
        /// </remarks>
        public static IService? Get(Type type)
        {
            if (m_Services.TryGetValue(type, out ActiveService entry) && entry.GetType() == type)
            {
                return entry.Service;
            }

            return default;
        }




        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{TService}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> even if service exist, but its type is wrong.
        /// </remarks>
        public static bool TryGet<T>([NotNullWhen(true)] out T? service) where T : class, IService
        {
            if (m_Services.TryGetValue(typeof(T), out ActiveService entry) && entry.Service is T t)
            {
                service = t;
                return true;
            }

            service = default;
            return false;
        }

        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{TService}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> even if service exist, but its type is wrong.
        /// </remarks>
        public static bool TryGet(Type type, [NotNullWhen(true)] out IService? service)
        {
            if (m_Services.TryGetValue(type, out ActiveService entry) && entry.Service.GetType() == type)
            {
                service = entry.Service;
                return true;
            }

            service = default;
            return false;
        }
    }
}
