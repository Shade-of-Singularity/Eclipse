using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Lists all <see cref="ServiceDescriptor"/>s constructed at runtime.
    /// </summary>
    public static class ServiceDescriptors
    {
        static readonly ConcurrentDictionary<Type, ServiceDescriptor> Descriptors = new();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ServiceDescriptor Get<T>() where T : IService => Get(typeof(T));
        public static ServiceDescriptor Get(Type type)
        {
            throw new NotImplementedException();
        }

        public static ServiceDescriptor GetOrAdd<T>() where T : IService => Get(typeof(T));
    }
}
