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
    /// </summary>
    /// <remarks>
    /// Right now only describes <paramref name="Getter"/> and <paramref name="Setter"/> for <see cref="IService{T}.Instance"/>.
    /// </remarks>
    /// <param name="Type">Type of our <see cref="IService"/>.</param>
    /// <param name="Getter">Getter for <see cref="IService{T}.Instance"/> property.</param>
    /// <param name="Setter">Setter for <see cref="IService{T}.Instance"/> property.</param>
    /// <param name="Associations">Other types, associated with this <see cref="IService"/>.</param>
    public sealed record class ServiceDescriptor(Type Type, ServiceGetter Getter, ServiceSetter Setter, Type[] Associations)
    {
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
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly Dictionary<Type, ServiceDescriptor> m_CachedDescriptors = new(64);
        private static readonly List<Type> m_AssociationsBuffer = new(16);
        private static readonly object _lock = new();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attempts to retrieve <see cref="ServiceDescriptor"/> from an internal cache.
        /// </summary>
        /// <param name="descriptor"><see cref="ServiceDescriptor"/> describing all service associations and some additional data.</param>
        /// <typeparam name="T">Type of the service to fetch a <see cref="ServiceDescriptor"/> for.</typeparam>
        /// <returns><c>true</c> if found. <c>false</c> if otherwise (service was never initialized)</returns>
        public static bool TryGetCached<T>(out ServiceDescriptor descriptor) where T : IService
        {
            lock (_lock) return m_CachedDescriptors.TryGetValue(typeof(T), out descriptor);
        }

        /// <summary>
        /// Attempts to retrieve <see cref="ServiceDescriptor"/> from an internal cache.
        /// </summary>
        /// <param name="type">Type of the service to retrieve an <see cref="ServiceDescriptor"/> for.</param>
        /// <param name="descriptor"><see cref="ServiceDescriptor"/> describing all service associations and some additional data.</param>
        /// <returns><c>true</c> if found. <c>false</c> if otherwise (service was never initialized)</returns>
        public static bool TryGetCached(Type type, [NotNullWhen(true)] out ServiceDescriptor? descriptor)
        {
            lock (_lock) return m_CachedDescriptors.TryGetValue(type, out descriptor);
        }

        /// <inheritdoc cref="Retrieve(Type, ServiceGetter, ServiceSetter)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Retrieve<T>(ServiceGetter getter, ServiceSetter setter) where T : IService
        {
            return Retrieve(typeof(T), getter, setter);
        }

        /// <summary>
        /// Retrieves <see cref="ServiceDescriptor"/> from internal cache, or construct a new one from the input data.
        /// </summary>
        /// <remarks>
        /// If target serviceType has <see cref="IgnoreServiceChildrenAttribute"/> defined (or inherited) - descriptor won't build associations.
        /// </remarks>
        /// <param name="serviceType">Type of our <see cref="IService"/>.</param>
        /// <param name="getter">Getter for <see cref="IService{T}.Instance"/> property.</param>
        /// <param name="setter">Setter for <see cref="IService{T}.Instance"/> property.</param>
        /// <returns>New <see cref="ServiceDescriptor"/> instance with information about our <see cref="IService"/>.</returns>
        public static ServiceDescriptor Retrieve(Type serviceType, ServiceGetter getter, ServiceSetter setter)
        {
            lock (_lock)
            {
                if (m_CachedDescriptors.TryGetValue(serviceType, out ServiceDescriptor result))
                {
                    // Partially updates descriptor if Getter or Setter delegates were changed.
                    if (result.Getter == getter && result.Setter == setter)
                    {
                        result = new(serviceType, getter, setter, result.Associations);
                        m_CachedDescriptors[serviceType] = result;
                    }

                    return result;
                }

                // Ignores all children of one of the parent services if requested.
                if (serviceType.IsDefined(typeof(IgnoreServiceChildrenAttribute), inherit: true))
                {
                    result = new(serviceType, getter, setter, serviceType.IsDefined(typeof(IgnoreServiceAttribute), inherit: false) ? [] : [serviceType]);
                    m_CachedDescriptors[serviceType] = result;
                    return result;
                }

                // Registers all interfaces implementing this serviceType.
                m_AssociationsBuffer.Clear();
                serviceType.FindInterfaces(Filter, null);

                // Registers all classes on the way to the base.
                Type type = serviceType;
                while (true)
                {
                    // Ignore types which ask for it.
                    if (!type.IsDefined(typeof(IgnoreServiceAttribute), inherit: false))
                    {
                        m_AssociationsBuffer.Add(type);
                    }

                    type = type.BaseType;
                    if (type is null || type == typeof(object))
                    {
                        result = new(serviceType, getter, setter, [.. m_AssociationsBuffer]);
                        m_CachedDescriptors[serviceType] = result;
                        return result;
                    }
                }
            }

            // Simplifications:
            static bool Filter(Type type, object? filter)
            {
                if (typeof(IService).IsAssignableFrom(type) && !type.IsDefined(typeof(IgnoreServiceAttribute), inherit: false))
                {
                    m_AssociationsBuffer.Add(type);
                }

                // Always return false, to not form an internal array.
                // TODO: Check source code to see how much resources, if any, this thing eats on idle run.
                return false;
            }
        }
    }
}
