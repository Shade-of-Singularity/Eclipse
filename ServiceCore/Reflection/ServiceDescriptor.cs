using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ServiceCore.Reflection
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
        private static readonly List<Type> m_AssociationsBuffer = new(16);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="Construct(Type, ServiceGetter, ServiceSetter, List{Type})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Construct<T>(ServiceGetter getter, ServiceSetter setter) where T : IService
        {
            lock (m_AssociationsBuffer) return Construct(typeof(T), getter, setter, m_AssociationsBuffer);
        }

        /// <inheritdoc cref="Construct(Type, ServiceGetter, ServiceSetter, List{Type})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Construct(Type type, ServiceGetter getter, ServiceSetter setter)
        {
            lock (m_AssociationsBuffer) return Construct(type, getter, setter, m_AssociationsBuffer);
        }

        /// <inheritdoc cref="Construct(Type, ServiceGetter, ServiceSetter, List{Type})"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Construct<T>(ServiceGetter getter, ServiceSetter setter, List<Type> associationsBuffer) where T : IService
        {
            return Construct(typeof(T), getter, setter, associationsBuffer);
        }

        /// <summary>
        /// Constructs <see cref="ServiceDescriptor"/> from simple input data.
        /// </summary>
        /// <remarks>
        /// If target serviceType has <see cref="IgnoreServiceChildrenAttribute"/> defined (or inherited) - descriptor won't build associations.
        /// </remarks>
        /// <param name="serviceType">Type of our <see cref="IService"/>.</param>
        /// <param name="getter">Getter for <see cref="IService{T}.Instance"/> property.</param>
        /// <param name="setter">Setter for <see cref="IService{T}.Instance"/> property.</param>
        /// <param name="associationsBuffer">Buffer for all associations with target serviceType, to be used in multi-threading.</param>
        /// <returns>New <see cref="ServiceDescriptor"/> instance with information about our <see cref="IService"/>.</returns>
        public static ServiceDescriptor Construct(Type serviceType, ServiceGetter getter, ServiceSetter setter, List<Type> associationsBuffer)
        {
            // Ignores all children of one of the parent services if requested.
            if (serviceType.IsDefined(typeof(IgnoreServiceChildrenAttribute), inherit: true))
            {
                return new(serviceType, getter, setter, serviceType.IsDefined(typeof(IgnoreServiceAttribute), inherit: false) ? [] : [serviceType]);
            }

            // Registers all interfaces implementing this serviceType.
            associationsBuffer.Clear();
            serviceType.FindInterfaces(Filter, associationsBuffer);

            // Registers all classes on the way to the base.
            Type type = serviceType;
            while (true)
            {
                // Ignore types which ask for it.
                if (!type.IsDefined(typeof(IgnoreServiceAttribute), inherit: false))
                {
                    associationsBuffer.Add(type);
                }

                type = type.BaseType;
                if (type is null || type == typeof(object))
                {
                    return new(serviceType, getter, setter, [.. associationsBuffer]);
                }
            }

            // Simplifications:
            static bool Filter(Type type, object list)
            {
                if (typeof(IService).IsAssignableFrom(type) && !type.IsDefined(typeof(IgnoreServiceAttribute), inherit: false))
                {
                    ((List<Type>)list).Add(type);
                }

                // Always return false, to not form an internal array.
                // TODO: Check source code to see how much resources, if any, this thing eats on idle run.
                return false;
            }
        }
    }
}
