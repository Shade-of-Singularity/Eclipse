//using System.Runtime.CompilerServices;

namespace ServiceCore
{
    /// <summary>
    /// Useful or Quality-of-Life extensions for <see cref="IService"/>.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// (Cached via CRTP) Retrieves <see cref="ServiceDescriptor"/> from <see cref="IService{T}.Descriptor"/> field directly.
        /// </summary>
        /// <param name="service">Service to retrieve a <see cref="ServiceDescriptor"/> for.</param>
        /// <returns><see cref="ServiceDescriptor"/> describing provided <paramref name="service"/>.</returns>
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static ServiceDescriptor GetDescriptor(this IService service) => service.Descriptor;
    }
}
