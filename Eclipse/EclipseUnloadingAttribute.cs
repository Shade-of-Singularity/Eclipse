using System;

namespace Eclipse
{
    /// <summary>
    /// Runs methods with this attribute when <see cref="Eclipse"/>.<see cref="Engine"/> unloads from the memory.
    /// </summary>
    /// <remarks>
    /// Unless manually requested to be unloaded, unloads only on <see cref="UnityEngine.Application.quitting"/>.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class EclipseUnloadingAttribute : Attribute
    {
        /// <summary>
        /// When attribute should be employed.
        /// </summary>
        public readonly UnloadingTiming Timing;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default constructor. Runs method in the latest possible point during engine unloading.
        /// <para>See also: <see cref="UnloadingTiming.AfterEngineUnloading"/>.</para>
        /// </summary>
        public EclipseUnloadingAttribute() : this(UnloadingTiming.AfterEngineUnloading) { }

        /// <summary>
        /// Full constructor. Allows you to specify when exactly attribute will used.
        /// </summary>
        /// <param name="timing"></param>
        public EclipseUnloadingAttribute(UnloadingTiming timing)
        {
            Timing = timing;
        }
    }
}
