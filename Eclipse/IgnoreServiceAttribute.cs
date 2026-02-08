using System;

namespace Eclipse
{
    /// <summary>
    /// Defines which services (or service interfaces) should be ignored when constructing service associations.
    /// </summary>
    /// <seealso cref="Services.ServiceEntry.Construct(IService)"/>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public sealed class IgnoreServiceAttribute : Attribute { }
}
