using System;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Classes with this attribute will run their class constructor
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SettingsAttribute : Attribute { }
}
