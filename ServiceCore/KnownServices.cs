using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Stores descriptors, associated with specific classes.
    /// Introduced to reduce make lookups faster, if service implements multiple <see cref="IService"/> interfaces (or interface + class, etc.)
    /// </summary>
    public static class KnownServices
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly ConcurrentDictionary<Type, ServiceDescriptor[]> m_Descriptors = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc cref="Get(Type)"/>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor[] Get<T>() where T : IService => Get(typeof(T));

        /// <summary>
        /// Retrieves all <see cref="ServiceDescriptor"/>s defined by a specific <see cref="IService"/> implementation.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ServiceDescriptor[] Get(Type type)
        {
            if (type is null) throw new ArgumentNullException(nameof(type));
            return m_Descriptors.GetOrAdd(type, static (type) =>
            {
                // Makes sure all service declarations create a descriptor about them.
                var interfaces = type.GetInterfaces();
                for (int i = 0; i < interfaces.Length; i++)
                {
                    RuntimeHelpers.RunClassConstructor(interfaces[i].TypeHandle);
                }

                Type temp = type;
                do
                {
                    RuntimeHelpers.RunClassConstructor(temp.TypeHandle);
                    temp = temp.BaseType;
                }
                while (temp is not null);

                // Fetches all associations.
                List<ServiceDescriptor> descriptors = [];
                for (int i = 0; i < interfaces.Length; i++)
                {
                    if (interfaces[i].IsDefined() && ServiceDescriptor.TryGetCached(interfaces[i], out var descriptor))
                    {
                        descriptors.Add(descriptor);
                    }
                }

                temp = type;
                do
                {
                    if (ServiceDescriptor.TryGetCached(interfaces[i], out var descriptor))
                    {
                        descriptors.Add(descriptor);
                    }

                    temp = temp.BaseType;
                }
                while (temp is not null);

                return null;
            });
        }
    }
}
