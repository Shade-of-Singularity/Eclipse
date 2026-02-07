using System;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Classes with this attribute will run their static class constructor during <see cref="ReconfigurationService"/> initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class SettingsAttribute : Attribute { }
}
