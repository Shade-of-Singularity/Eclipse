using System;

namespace Eclipse
{
    /// <summary>
    /// Forces static class to run its static .ctor with <see cref="Eclipse"/>.<see cref="Engine"/>. 
    /// </summary>
    /// <remarks>
    /// Static .ctor will run after <see cref="EngineService"/>s were instantiated, but BEFORE they were initialized.
    /// Read this as - it will be dangerous to interact with <see cref="EngineService{T}.Instance"/>s at this point.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class EclipseInitializeAttribute : Attribute
    {
        /// <summary>
        /// When attribute should be employed.
        /// </summary>
        public readonly InitializationTiming Timing;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Default constructor. Runs method in the latest possible point during engine initialization.
        /// <para>See also: <see cref="InitializationTiming.AfterEngineInitialization"/>.</para>
        /// </summary>
        public EclipseInitializeAttribute() : this(InitializationTiming.AfterEngineInitialization) { }

        /// <summary>
        /// Full constructor. Allows you to specify when exactly attribute will used.
        /// </summary>
        /// <param name="timing"></param>
        public EclipseInitializeAttribute(InitializationTiming timing)
        {
            Timing = timing;
        }
    }
}
