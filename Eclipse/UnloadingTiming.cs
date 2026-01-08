using System;
using System.Collections.Generic;
using System.Text;

namespace Eclipse
{
    /// <summary>
    /// Indicates when <see cref="EclipseUnloadingAttribute"/> should run underlying method.
    /// </summary>
    /// <remarks>
    /// 
    /// </remarks>
    /// Also, uses <see cref="ushort"/> instead of <see cref="byte"/> in case we will need 10-12 callbacks.
    /// TODO: Make callback system in <see cref="Engine"/> which allows you to subscribe to each unloading timing during Engine unloading.
    public enum UnloadingTiming : ushort
    {
        /// <summary>
        /// Unloading method is never run.
        /// </summary>
        Never = 0,

        /// <summary>
        /// Runs before anything was unloaded from the engine.
        /// </summary>
        BeforeEngineUnloading = 0b0000_0000_0000_0001,

        /// <summary>
        /// Runs after entire engine was unloaded.
        /// </summary>
        AfterEngineUnloading = 0b1000_0000_0000_0000,
    }
}
