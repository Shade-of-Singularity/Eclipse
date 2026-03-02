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
        private static readonly ConcurrentDictionary<Type, ServiceDescriptor?> m_CachedDescriptors = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="Construct(Type, ServiceGetter, ServiceSetter)"/>
        /// <typeparam name="T"><see cref="IService"/> which defines <see cref="ServiceIdentifierAttribute"/>.</typeparam>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Construct<T>(ServiceGetter getter, ServiceSetter setter) where T : IService
        {
            return Construct(typeof(T), getter, setter);
        }

        /// <summary>
        /// Constructs new instance of <see cref="ServiceDescriptor"/> for given <paramref name="serviceType"/>.
        /// </summary>
        /// <param name="serviceType"><see cref="IService"/> which defines <see cref="ServiceIdentifierAttribute"/>.</param>
        /// <param name="getter">Getter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <param name="setter">Setter for service instance property (e.g. <see cref="IService{T}.Instance"/> or <see cref="Service{T}.Instance"/>).</param>
        /// <returns>New <see cref="ServiceDescriptor"/> for a given <paramref name="serviceType"/>.</returns>
        /// <exception cref="Exception">Indicates that provided <paramref name="serviceType"/> doesn't define <see cref="ServiceIdentifierAttribute"/>.</exception>
        public static ServiceDescriptor Construct(Type serviceType, ServiceGetter getter, ServiceSetter setter)
        {
            if (!serviceType.IsDefined(typeof(ServiceIdentifierAttribute), inherit: false))
            {
                throw new Exception($"Type {serviceType} does not define {nameof(ServiceIdentifierAttribute)}. {nameof(ServiceDescriptor)} won't be built.");
            }

            return new ServiceDescriptor(serviceType, getter, setter);
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
