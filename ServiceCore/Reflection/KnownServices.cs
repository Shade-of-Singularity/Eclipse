using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using static ServiceCore.Services;

namespace ServiceCore.Reflection
{
    /// <summary>
    /// Stores reflection information about known <see cref="IService"/>s.
    /// </summary>
    /// <remarks>
    /// This information persists even if <see cref="Modding.Modification"/> containing a service is no longer loaded.
    /// </remarks>
    public static class KnownServices // Note: Consider making it internal.
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        static readonly Dictionary<Type, ServiceDescriptor> Services = [];




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Register information about specific <see cref="IService"/> under type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of a specified <see cref="IService"/>.</typeparam>
        /// <param name="descriptor">Information about specified <see cref="IService"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Register<T>(ServiceDescriptor descriptor) where T : IService => Register(typeof(T), descriptor);

        /// <summary>
        /// Register information about specific <see cref="IService"/> <paramref name="type"/>.
        /// </summary>
        /// <param name="type">Type of an <see cref="IService"/>.</param>
        /// <param name="descriptor">Information about specified <see cref="IService"/>.</param>
        public static void Register(Type type, ServiceDescriptor descriptor)
        {
            lock (Services)
            {
                // Registers service.
                Type[] associations = descriptor.Associations;
                for (int i = 0; i < associations.Length; i++)
                {
                    Type association = associations[i];
                    if (association is null) continue;

                    if (!Services.TryAdd(association, descriptor))
                    {
                        // Overwrites previously existing service entirely.
                        ServiceDescriptor existing = Services[association];
                        Array.ForEach(existing.Associations, static a => Services.Remove(a));
                        Services[association] = descriptor;
                    }
                }
            }
        }

        /// <summary>
        /// Retrieves information about specified <see cref="IService"/>.
        /// </summary>
        /// <typeparam name="T">Type of required <see cref="IService"/>.</typeparam>
        /// <returns>Information about specified <see cref="IService"/>.</returns>
        public static ServiceDescriptor? Retrieve<T>() where T : IService
        {
            lock (Services) return Services.GetValueOrDefault(typeof(T));
        }

        /// <summary>
        /// Retrieves information about specified <see cref="IService"/>.
        /// </summary>
        /// <param name="type">Type of required <see cref="IService"/>.</param>
        /// <returns>Information about specified <see cref="IService"/>.</returns>
        public static ServiceDescriptor? Retrieve(Type type)
        {
            lock (Services) return Services.GetValueOrDefault(type);
        }

        /// <summary>
        /// Tries to retrieve information about specified <see cref="IService"/>.
        /// </summary>
        /// <typeparam name="T">Type of required <see cref="IService"/>.</typeparam>
        /// <param name="descriptor">Information about specified <see cref="IService"/>.</param>
        /// <returns><c>true</c> when <paramref name="descriptor"/> were found and it is provided. <c>false</c> otherwise.</returns>
        public static bool TryRetrieve<T>([NotNullWhen(true)] out ServiceDescriptor? descriptor) where T : IService
        {
            lock (Services) return Services.TryGetValue(typeof(T), out descriptor);
        }

        /// <summary>
        /// Tries to retrieve information about specified <see cref="IService"/>.
        /// </summary>
        /// <param name="type">Type of required <see cref="IService"/>.</param>
        /// <param name="descriptor">Information about specified <see cref="IService"/>.</param>
        /// <returns><c>true</c> when <paramref name="descriptor"/> were found and it is provided. <c>false</c> otherwise.</returns>
        public static bool Retrieve(Type type, [NotNullWhen(true)] out ServiceDescriptor? descriptor)
        {
            lock (Services) return Services.TryGetValue(type, out descriptor);
        }
    }
}
