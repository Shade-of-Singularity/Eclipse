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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Delegate, describing <see cref="IService{T}.Instance"/> getter.
    /// </summary>
    /// <returns>Retrieved serviceIdentifier.</returns>
    public delegate IService? ServiceGetter();

    /// <summary>
    /// Delegate, descripting <see cref="IService{T}.Instance"/> setter.
    /// </summary>
    /// <param name="service">Service to set to underlying field of <see cref="IService{T}.Instance"/>.</param>
    public delegate void ServiceSetter(IService? service);

    /// <summary>
    /// Describes an serviceIdentifier <see cref="IService"/>.
    /// Similar to <see cref="Type"/> in its essence.
    /// </summary>
    public sealed class ServiceDescriptor : IEquatable<ServiceDescriptor>
    {
        /// <summary>
        /// <see cref="ServiceDescriptor"/> serviceIdentifier for all invalid descriptors.
        /// </summary>
        /// <remarks>
        /// Services without <see cref="ServiceIdentifierAttribute"/> in the inheritance tree are marked as such.
        /// </remarks>
        //public static readonly ServiceDescriptor Invalid = new(null!, null!, null!, []);

        /// <summary>
        /// Type implementing <see cref="IService"/> with <see cref="ServiceIdentifierAttribute"/> defined.
        /// </summary>
        public Type Identifier { get; }

        /// <summary>
        /// Getter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).
        /// </summary>
        [NotNullIfNotNull(nameof(Identifier))] public ServiceGetter Getter { get; }

        /// <summary>
        /// Setter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).
        /// </summary>
        /// TODO: Add "OnServiceChanged" callback, but make it pending if <see cref="Services.Unsafe.Initialize()"/> lock is active.
        [NotNullIfNotNull(nameof(Identifier))] public ServiceSetter Setter { get; }

        /// <summary>
        /// Whether or not this serviceIdentifier stays intact regardless of <see cref="Engine"/> initialization/termination.
        /// </summary>
        /// <remarks>
        /// <see cref="BeforeServiceInitializedAttribute"/> and <see cref="AfterServiceInitializedAttribute"/> callbacks
        /// will fire immediately on <see cref="IService{T}.Instantiate{TService}(IInitializationArgs?)"/> or similar methods.
        /// </remarks>
        public bool Persistent { get; set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ServiceDescriptor(Type identifier, ServiceGetter getter, ServiceSetter setter)
        {
            Identifier = identifier;
            Getter = getter;
            Setter = setter;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly ConcurrentDictionary<Type, ServiceDescriptor> m_Descriptors = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="Construct(Type, ServiceGetter, ServiceSetter)"/>
        /// <typeparam name="T"><see cref="IService"/> which defines <see cref="ServiceIdentifierAttribute"/>.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Construct<T>(ServiceGetter getter, ServiceSetter setter) where T : class, IService
        {
            return Construct(typeof(T), getter, setter);
        }

        /// <summary>
        /// Constructs new instance of <see cref="ServiceDescriptor"/> for given <paramref name="serviceIdentifier"/>.
        /// </summary>
        /// <param name="serviceIdentifier">Type of <see cref="IService"/> which explicitly defines <see cref="ServiceIdentifierAttribute"/>.</param>
        /// <param name="getter">Getter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <param name="setter">Setter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <returns>New <see cref="ServiceDescriptor"/> for a given <paramref name="serviceIdentifier"/>.</returns>
        /// <exception cref="Exception">Indicates that provided <paramref name="serviceIdentifier"/> doesn't define <see cref="ServiceIdentifierAttribute"/>.</exception>
        public static ServiceDescriptor Construct(Type serviceIdentifier, ServiceGetter getter, ServiceSetter setter)
        {
            if (!serviceIdentifier.IsDefined(typeof(ServiceIdentifierAttribute), inherit: false))
            {
                throw new Exception($"{Engine.LogPrefix} Type {serviceIdentifier} does not define {nameof(ServiceIdentifierAttribute)}. {nameof(ServiceDescriptor)} cannot be built.");
            }

            ServiceDescriptor descriptor = new(serviceIdentifier, getter, setter);
            if (m_Descriptors.TryAdd(serviceIdentifier, descriptor))
            {
                return descriptor;
            }

            throw new Exception($"{Engine.LogPrefix} Type {serviceIdentifier} attempted to construct {nameof(ServiceDescriptor)} twice.");
        }

        /// <inheritdoc cref="Get(Type)"/>
        /// <typeparam name="T"><see cref="IService"/> which defines <see cref="ServiceIdentifierAttribute"/>.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Get<T>() where T : class, IService
        {
            return Get(typeof(T));
        }

        /// <summary>
        /// Retrieves <see cref="ServiceDescriptor"/> for a given <paramref name="serviceIdentifier"/> type.
        /// </summary>
        /// <param name="serviceIdentifier">Type of <see cref="IService"/> which explicitly defines <see cref="ServiceIdentifierAttribute"/>.</param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public static ServiceDescriptor Get(Type serviceIdentifier)
        {
            if (!m_Descriptors.TryGetValue(serviceIdentifier, out ServiceDescriptor? descriptor))
            {
                if (!typeof(IService).IsAssignableFrom(serviceIdentifier))
                {
                    throw new Exception($"{Engine.LogPrefix} Cannot retrieve {nameof(ServiceDescriptor)} for identifier ({serviceIdentifier.Name}), which doesn't implement {nameof(IService)}");
                }

                if (!serviceIdentifier.IsDefined(typeof(ServiceIdentifierAttribute), inherit: false))
                {
                    throw new Exception($"{Engine.LogPrefix} Identifier ({serviceIdentifier.Name}) doesn't define {nameof(ServiceIdentifierAttribute)}. Cannot get {nameof(ServiceDescriptor)} for it.");
                }

                throw new Exception($"{Engine.LogPrefix} Identifier ({serviceIdentifier.Name}) define {nameof(ServiceIdentifierAttribute)} but haven't constructed {nameof(ServiceDescriptor)}. Cannot get {nameof(ServiceDescriptor)} for it");
            }

            return descriptor;
        }

        /// <inheritdoc cref="TryGet(Type, out ServiceDescriptor?)"/>
        /// <typeparam name="T"><see cref="IService"/> which defines <see cref="ServiceIdentifierAttribute"/>.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGet<T>([NotNullWhen(true)] out ServiceDescriptor? descriptor) where T : class, IService
        {
            return TryGet(typeof(T), out descriptor);
        }

        /// <summary>
        /// Retrieves already constructed <see cref="ServiceDescriptor"/> about specific <see cref="IService"/>.
        /// </summary>
        /// <param name="serviceIdentifier">Type of <see cref="IService"/> which explicitly defines <see cref="ServiceIdentifierAttribute"/>.</param>
        /// <param name="descriptor"><see cref="ServiceDescriptor"/> for a given <paramref name="serviceIdentifier"/>.</param>
        /// <returns><c>true</c> if <paramref name="descriptor"/> were found. <c>false</c> otherwise.</returns>
        public static bool TryGet(Type serviceIdentifier, [NotNullWhen(true)] out ServiceDescriptor? descriptor)
        {
            return m_Descriptors.TryGetValue(serviceIdentifier, out descriptor);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public bool Equals(ServiceDescriptor other) => other is not null && other.Identifier == Identifier;

        /// <inheritdoc/>
        public override int GetHashCode() => Identifier.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is ServiceDescriptor descriptor && descriptor.Identifier == Identifier;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{Identifier} (Getter: {Getter}) (Setter: {Setter})";
        }
    }
}
