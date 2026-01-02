using Eclipse.Configuration;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Eclipse
{
    /// <summary>
    /// Stores information about system specs of current device.
    /// Information will be used for networking and various optimization techniques.
    /// </summary>
    /// <remarks>
    /// Everything that is not readonly can be modified at runtime.
    /// Everything that is readonly updated on application start.
    /// </remarks>
    public static partial class Specs
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static readonly bool IsDesktop = SystemInfo.deviceType == DeviceType.Desktop;
        public static readonly bool IsHandheld = SystemInfo.deviceType == DeviceType.Handheld;
        public static readonly bool IsConsole = SystemInfo.deviceType == DeviceType.Console;
        public static readonly bool IsUnknownDevice = SystemInfo.deviceType == DeviceType.Unknown;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Extra
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static class Memory
        {
            /// <summary>
            /// Called whenever <see cref="L1Memory"/>, <see cref="L1Memory"/> or <see cref="L1Memory"/> was changed and applied.
            /// </summary>
            /// <remarks>
            /// Also called when <see cref=""/>
            /// </remarks>
            public static event Action OnMemorySpecsChanged;

            /// <summary>
            /// Whether any values for L1, L2 and L3 memory was specified.
            /// </summary>
            public static bool LMemorySpecified => false;

            /// <summary>
            /// L1 Memory size in bytes, specified in game settings.
            /// </summary>
            public static int L1Memory
            {
                get => m_L1Memory;
                set => 




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static int m_L1Memory;
        }

        public static class Disk
        {
            /// <summary>
            /// Wherever supported, will use a more disk-space hungry saving methods, but will cause less writing to a disk overall, making it wear down slower.
            /// </summary>
            /// <remarks>
            /// Real impacts of this option are untested yet.
            /// </remarks>
            public bool ReduceDiskWear
            {
                get => m_ReduceDiskWear;
                set => 
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Constructors
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            static Disk()
            {

            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            private static bool m_ReduceDiskWear;
        }
    }
}
