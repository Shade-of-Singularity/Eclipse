using System;

namespace ServiceCore
{
    /// <summary>
    /// Do not use this class unless you know what you are doing! (or unless you are writing your own CRTP ServiceCore extension)
    /// Marks class/instance (e.g. <see cref="Service{T}"/> or <see cref="IService{T}"/>) as an identifier in inheritance tree.
    /// </summary>
    /// <remarks>
    /// Interfaces are prioritized over classes (e.g. <see cref="IService{T}"/> prioritized over <see cref="Service{T}"/>)
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class ServiceIdentifierAttribute : Attribute { }
}
