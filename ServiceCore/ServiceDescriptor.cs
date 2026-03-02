using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Delegate, describing <see cref="IService{T}.Instance"/> getter.
    /// </summary>
    /// <returns>Retrieved serviceType.</returns>
    public delegate IService? ServiceGetter();

    /// <summary>
    /// Delegate, descripting <see cref="IService{T}.Instance"/> setter.
    /// </summary>
    /// <param name="service">Service to set to underlying field of <see cref="IService{T}.Instance"/>.</param>
    public delegate void ServiceSetter(IService? service);

    /// <summary>
    /// Describes an serviceType <see cref="IService"/>.
    /// Similar to <see cref="Type"/> in its essence.
    /// </summary>
    public sealed class ServiceDescriptor : IEquatable<ServiceDescriptor>
    {
        /// <summary>
        /// <see cref="ServiceDescriptor"/> serviceType for all invalid descriptors.
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
        [NotNullIfNotNull(nameof(Identifier))] public ServiceSetter Setter { get; }

        /// <summary>
        /// Whether or not this serviceType stays intact regardless of <see cref="Engine"/> initialization/termination.
        /// </summary>
        /// <remarks>
        /// <see cref="BeforeServiceInitializedAttribute"/> and <see cref="AfterServiceInitializedAttribute"/> callbacks
        /// will fire immediately on <see cref="IService{T}.Instantiate{TService}(IInitializationArgs?)"/> or similar methods.
        /// </remarks>
        public bool Persistent { get; set; }

        /// <summary>
        /// Types associated with 
        /// </summary>
        public Type[] Associations
        {
            get => m_Associations is not null ? m_Associations : (m_Associations = GetAssociations(Identifier));
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private Type[]? m_Associations;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private ServiceDescriptor(Type identifier, ServiceGetter getter, ServiceSetter setter, Type[]? associations)
        {
            Identifier = identifier;
            Getter = getter;
            Setter = setter;
            m_Associations = associations;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, ServiceDescriptor?> m_CachedDescriptors = new(64);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static Type[] GetAssociations(Type identifier)
        {
            if (!typeof(IService).IsAssignableFrom(identifier))
            {
                return [];
            }

            List<Type> associations = [];
            var interfaces = identifier.GetInterfaces();
            for (int i = 0; i < interfaces.Length; i++)
            {
                var temp = interfaces[i];
                if (typeof(IService).IsAssignableFrom(temp) && !temp.IsDefined(typeof(DoNotAssociate), inherit: false))
                {
                    associations.Add(temp);
                }
            }

            do
            {
                if (!identifier.IsDefined(typeof(DoNotAssociate), inherit: false))
                {
                    associations.Add(identifier);
                }

                identifier = identifier.BaseType;
            }
            while (typeof(IService).IsAssignableFrom(identifier) && identifier != typeof(object));
            return [.. associations];
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="TryGetCached(Type, out ServiceDescriptor?)"/>
        /// <typeparam name="T">Identifier of the service to fetch a <see cref="ServiceDescriptor"/> for.</typeparam>
        public static bool TryGetCached<T>([NotNullWhen(true)] out ServiceDescriptor? descriptor) where T : IService
        {
            lock (m_CachedDescriptors) return m_CachedDescriptors.TryGetValue(typeof(T), out descriptor) && descriptor is not null;
        }

        /// <summary>
        /// Attempts to retrieve <see cref="ServiceDescriptor"/> from an internal cache.
        /// </summary>
        /// <param name="serviceType">Identifier of the service to retrieve an <see cref="ServiceDescriptor"/> for.</param>
        /// <param name="descriptor"><see cref="ServiceDescriptor"/> describing all service associations and some additional data.</param>
        /// <returns><c>true</c> if found. <c>false</c> if otherwise (service was never initialized)</returns>
        public static bool TryGetCached(Type serviceType, [NotNullWhen(true)] out ServiceDescriptor? descriptor)
        {
            lock (m_CachedDescriptors) return m_CachedDescriptors.TryGetValue(serviceType, out descriptor) && descriptor is not null;
        }




        /// <inheritdoc cref="GetCached(Type)"/>
        /// <typeparam name="T">Identifier of the service to fetch a <see cref="ServiceDescriptor"/> for.</typeparam>
        public static ServiceDescriptor? GetCached<T>() where T : IService
        {
            lock (m_CachedDescriptors) return m_CachedDescriptors.GetValueOrDefault(typeof(T));
        }

        /// <summary>
        /// Retrieves <see cref="ServiceDescriptor"/> from an internal cache.
        /// </summary>
        /// <param name="serviceType">Identifier of the service to retrieve an <see cref="ServiceDescriptor"/> for.</param>
        /// <returns><c>true</c> if found. <c>false</c> if otherwise (service was never initialized)</returns>
        public static ServiceDescriptor? GetCached(Type serviceType)
        {
            lock (m_CachedDescriptors) return m_CachedDescriptors.GetValueOrDefault(serviceType);
        }




        /// <inheritdoc cref="TryRetrieve(Type, ServiceGetter, ServiceSetter, out ServiceDescriptor?)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRetrieve<T>(ServiceGetter getter, ServiceSetter setter, [NotNullWhen(true)] out ServiceDescriptor? descriptor) where T : IService
        {
            descriptor = Retrieve(typeof(T), getter, setter);
            return descriptor is not null;
        }

        /// <summary>
        /// Attempts to retrieve <see cref="ServiceDescriptor"/> from a provided <paramref name="serviceType"/> serviceType.
        /// Result is cached and will be returned on the next call.
        /// </summary>
        /// <remarks>
        /// <see cref="Getter"/> and <see cref="Setter"/> won't change if returned <paramref name="descriptor"/> has different delegates.
        /// </remarks>
        /// <param name="serviceType">Type which inherits <see cref="IService"/> and defines <see cref="ServiceIdentifierAttribute"/> somewhere.</param>
        /// <param name="getter">Getter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <param name="setter">Setter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <param name="descriptor">
        /// <c>null</c> if <paramref name="serviceType"/> doesn't inherit <see cref="IService"/> or doesn't define <see cref="ServiceIdentifierAttribute"/>.
        /// Otherwise, returns <see cref="ServiceDescriptor"/> describing <paramref name="serviceType"/>.
        /// </param>
        /// <returns>
        /// <c>true</c> if <see cref="ServiceDescriptor"/> was retrieved successfully.
        /// <c>false</c> if <paramref name="serviceType"/> doesn't implement <see cref="IService"/> and doesn't define <see cref="ServiceIdentifierAttribute"/>
        /// in base classes or implemented interfaces (Examples: <see cref="Service{T}"/> or <see cref="IService{T}"/>).
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryRetrieve(Type serviceType, ServiceGetter getter, ServiceSetter setter, [NotNullWhen(true)] out ServiceDescriptor? descriptor)
        {
            descriptor = Retrieve(serviceType, getter, setter);
            return descriptor is not null;
        }




        /// <inheritdoc cref="Retrieve(Type, ServiceGetter, ServiceSetter)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor? Retrieve<T>(ServiceGetter getter, ServiceSetter setter) where T : IService
        {
            return Retrieve(typeof(T), getter, setter);
        }

        /// <summary>
        /// Attempts to retrieve <see cref="ServiceDescriptor"/> from a provided <paramref name="serviceType"/> serviceType.
        /// Result is cached and will be returned on the next call.
        /// </summary>
        /// <remarks>
        /// <see cref="Getter"/> and <see cref="Setter"/> won't change if returned <see cref="ServiceDescriptor"/> has different delegates.
        /// </remarks>
        /// <param name="serviceType">Type which inherits <see cref="IService"/> and defines <see cref="ServiceIdentifierAttribute"/> somewhere.</param>
        /// <param name="getter">Getter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <param name="setter">Setter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <returns>
        /// <c>null</c> if <paramref name="serviceType"/> doesn't inherit <see cref="IService"/> or doesn't define <see cref="ServiceIdentifierAttribute"/>.
        /// Otherwise, returns <see cref="ServiceDescriptor"/> describing <paramref name="serviceType"/>.
        /// </returns>
        public static ServiceDescriptor? Retrieve(Type serviceType, ServiceGetter getter, ServiceSetter setter)
        {
            lock (m_CachedDescriptors)
            {
                // 1. Check cached declarations. Return them if they are present.
                if (m_CachedDescriptors.TryGetValue(serviceType, out ServiceDescriptor? descriptor))
                {
                    return descriptor;
                }

                // 2. Identify the identifier interface.
                // Note: With this implementation, one class cannot implement multiple interfaces.
                //  I wonder if it's a problem. In theory we can just create descriptors for multiple classes and store them internally.
                //  But one of them will remain inaccessible under such implementation.
                Type? identifier;
                var interfaces = serviceType.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    identifier = interfaces[i];
                    if (identifier.IsDefined(typeof(ServiceIdentifierAttribute), inherit: false))
                    {
                        descriptor = new(identifier, getter, setter, null);
                        m_CachedDescriptors[serviceType] = descriptor;
                        return descriptor;
                    }
                }

                // 3. If interface not found - identify the identifier class.
                identifier = serviceType;
                while (identifier is not null && identifier != typeof(object))
                {
                    if (identifier.IsDefined(typeof(ServiceIdentifierAttribute), inherit: false))
                    {
                        descriptor = new(identifier, getter, setter, null);
                        m_CachedDescriptors[serviceType] = descriptor;
                        return descriptor;
                    }

                    identifier = identifier.BaseType;
                }

                m_CachedDescriptors[serviceType] = null;
                return null;
            }
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
