using System;

namespace ServiceCore
{
    /// <summary>
    /// Signals to <see cref="ServiceDescriptor"/> to not associate class or interface
    /// defining this attribute in <see cref="ServiceDescriptor.Associations"/> collection.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class DoNotAssociate : Attribute { }
}
