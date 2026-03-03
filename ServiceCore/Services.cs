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
        /// Describes all services activated after <see cref="Engine.Initialize(InitializationContext?, IInitializationArgs?)"/> invocation.
        /// </summary>
        public static IEnumerable<ServiceDescriptor> RuntimeServices => m_RuntimeServices;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly HashSet<ServiceDescriptor> m_RuntimeServices = [];




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
        /// Note: At the moment, initializing <see cref="ServiceRange"/> on this call is the best approach.
        /// TODO: Replace with checks specific for runtime services, or make <see cref="Services"/> outline persistent services as well.
        public static bool Has<T>() where T : class, IService => !ServiceRange.Invalid.Equals(ServiceRanges.Retrieve(typeof(T)));

        /// <summary>
        /// Checks if there is a service with requested <paramref Identifier="type"/>.
        /// </summary>
        /// <remarks>
        /// Less optimized than strongly-typed <see cref="IService{TService}.Exist"/> or <see cref="Has{T}()"/>.
        /// </remarks>
        /// Note: At the moment, initializing <see cref="ServiceRange"/> on this call is the best approach.
        /// TODO: Replace with checks specific for runtime services, or make <see cref="Services"/> outline persistent services as well.
        public static bool Has(Type type) => !ServiceRange.Invalid.Equals(ServiceRanges.Retrieve(type));




        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Never throws. Instead, returns <c>null</c> if service is not defined or its type was changed.
        /// </remarks>
        /// TODO: Replace with checks specific for runtime services, or make <see cref="Services"/> outline persistent services as well.
        public static T? Get<T>() where T : class, IService => (T?)(ServiceRanges.Retrieve(typeof(T)).First?.Getter());

        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Never throws. Instead, returns <c>null</c> if service is not defined or its type was changed.
        /// </remarks>
        /// TODO: Replace with checks specific for runtime services, or make <see cref="Services"/> outline persistent services as well.
        public static IService? Get(Type type) => ServiceRanges.Retrieve(type).First?.Getter();




        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{T}.TryGet"/> or <see cref="Service{T}.TryGet"/> directly.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> even if service exist, but its type is wrong.
        /// </remarks>
        /// TODO: Replace with checks specific for runtime services, or make <see cref="Services"/> outline persistent services as well.
        public static bool TryGet<T>([NotNullWhen(true)] out T? service) where T : class, IService
        {
            return (service = (T?)(ServiceRanges<T>.Range.First?.Getter())) is not null;
        }

        /// <summary>
        /// Retrieves service of a requested type.
        /// More expensive than using <see cref="IService{TService}.Instance"/> directly.
        /// </summary>
        /// <remarks>
        /// Will return <c>false</c> even if service exist, but its type is wrong.
        /// </remarks>
        /// TODO: Replace with checks specific for runtime services, or make <see cref="Services"/> outline persistent services as well.
        public static bool TryGet(Type type, [NotNullWhen(true)] out IService? service)
        {
            return (service = (ServiceRanges.Retrieve(type).First?.Getter())) is not null;
        }
    }
}
