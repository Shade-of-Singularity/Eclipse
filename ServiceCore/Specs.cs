/// - - -    Copyright (c) 2025     - - -     SoG, DarkJune     - - - <![CDATA[
/// 
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
/// 
///         http://www.apache.org/licenses/LICENSE-2.0
/// 
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// ]]>

using ServiceCore.Configuration;
using System;
using System.IO;
using UnityEngine;

namespace ServiceCore
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
        /// <summary>
        /// Whether user system is a Desktop device (Windows, Linux, macOS, etc.)
        /// </summary>
        public static readonly bool IsDesktop = SystemInfo.deviceType == DeviceType.Desktop;

        /// <summary>
        /// Whether user system is a Handheld device (Android, iPhone, etc.)
        /// </summary>
        public static readonly bool IsHandheld = SystemInfo.deviceType == DeviceType.Handheld;

        /// <summary>
        /// Whether user system is a Console device (XBox, etc.)
        /// </summary>
        public static readonly bool IsConsole = SystemInfo.deviceType == DeviceType.Console;

        /// <summary>
        /// Any other type of device, other than <see cref="IsDesktop"/>, <see cref="IsHandheld"/> and <see cref="IsConsole"/>.
        /// </summary>
        public static readonly bool IsUnknownDevice = SystemInfo.deviceType == DeviceType.Unknown;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Extra
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Information about CPU caches.
        /// <para>
        /// Note: only access this section after <see cref="DefaultConfigurationService"/> has initialized.
        /// </para>
        /// </summary>
        /// <remarks>
        /// It is not recommended to refer to this class in loops and similar.
        /// Checking for cache usage every frame will probably consume too much CPU resources.
        /// In 99% of cases, you are better off just generally optimize your game - avoid calling Unity API frequently to avoid extern calls and similar.
        /// Use this class only to adjust how your systems behave in a long term, by changing references to the methods and functions used, etc.
        /// Reliable cache usage optimizations on all possible systems require a lot of expertise and knowledge,
        /// so be mindful about how you use data in this section. Good luck.
        /// </remarks>
        public static class Cache
        {
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                 Constants
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>            
            /// <summary>
            /// Key used to serialize data of <see cref="L1Cache"/> property.
            /// </summary>
            public const string L1CacheKey = "@" + nameof(L1Cache);

            /// <summary>
            /// Key used to serialize data of <see cref="L2Cache"/> property.
            /// </summary>
            public const string L2CacheKey = "@" + nameof(L2Cache);

            /// <summary>
            /// Key used to serialize data of <see cref="L3Cache"/> property.
            /// </summary>
            public const string L3CacheKey = "@" + nameof(L3Cache);

            /// <summary>
            /// Key used to serialize data of <see cref="UseCacheOptimizations"/> property.
            /// </summary>
            public const string UseCacheOptimizationsKey = "@" + nameof(UseCacheOptimizations);

            /// <summary>
            /// Key used to serialize data of <see cref="CacheUtilization"/> property.
            /// </summary>
            public const string CacheUtilizationKey = "@" + nameof(CacheUtilization);

            /// <summary>
            /// If L1 cache capacity is lower than this value - optimizations will be prohibited.
            /// Cache checks will probably hurt more at this point, if you have so little cache.
            /// </summary>
            public const uint LowerL1CacheBound = 1024 * 32; // 32KB

            /// <summary>
            /// If L2 cache capacity is lower than this value - optimizations will be prohibited.
            /// Cache checks will probably hurt more at this point, if you have so little cache.
            /// </summary>
            public const uint LowerL2CacheBound = 1024 * 64; // 64KB

            /// <summary>
            /// If L3 cache capacity is lower than this value - optimizations will be prohibited.
            /// Cache checks will probably hurt more at this point, if you have so little cache.
            /// </summary>
            public const uint LowerL3CacheBound = 1024 * 128; // 128KB




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                   Events
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Called whenever <see cref="FinalL1Cache"/>, <see cref="FinalL2Cache"/>,
            /// <see cref="FinalL3Cache"/>) or <see cref="PerformOptimizations"/>
            /// was changed and changes were applied.
            /// </summary>
            public static event Action? OnCacheSpecsChanged;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Public Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Whether any cache optimizations are allowed by current settings.
            /// </summary>
            public static bool PerformOptimizations { get; private set; }

            /// <summary>
            /// Total amount of cache on all levels.
            /// </summary>
            public static ulong TotalCacheSize { get; private set; }

            /// <summary>
            /// Final L1 cache size to use as a reference.
            /// </summary>
            public static ulong FinalL1Cache { get; private set; }

            /// <summary>
            /// Final L2 cache size to use as a reference.
            /// </summary>
            public static ulong FinalL2Cache { get; private set; }

            /// <summary>
            /// Final L3 cache size to use as a reference.
            /// </summary>
            public static ulong FinalL3Cache { get; private set; }

            /// <summary>
            /// Whether services, engine and game should attempt to use cache optimizations, if possible.
            /// </summary>
            public static bool UseCacheOptimizations
            {
                get => m_UseCacheOptimizations;
                set
                {
                    if (Configure.SetValue(UseCacheOptimizationsKey, ref m_UseCacheOptimizations, value))
                    {
                        PerformOptimizations = IsSettingsAllowOptimizations();
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeSpecsChangedCallback);
                    }
                }
            }

            /// <summary>
            /// Cache utilization in percent values.
            /// </summary>
            /// <remarks>
            /// It's recommended to round values to whole integers on game UI, for better user experience.
            /// </remarks>
            public static float CacheUtilization
            {
                get => m_CacheUtilization;
                set
                {
                    value = Math.Clamp(value, 0f, 1f);
                    if (Configure.SetValue(CacheUtilizationKey, ref m_CacheUtilization, value))
                    {
                        FinalL1Cache = (uint)Math.Ceiling(m_L1Cache * value);
                        FinalL2Cache = (uint)Math.Ceiling(m_L2Cache * value);
                        FinalL3Cache = (uint)Math.Ceiling(m_L3Cache * value);
                        TotalCacheSize = FinalL1Cache + FinalL2Cache + FinalL3Cache;
                        PerformOptimizations = IsSettingsAllowOptimizations();
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeSpecsChangedCallback);
                    }
                }
            }

            /// <summary>
            /// L1 Memory size in bytes, specified in advanced performance settings.
            /// </summary>
            public static ulong L1Cache
            {
                get => m_L1Cache;
                set
                {
                    if (Configure.SetValue(L1CacheKey, ref m_L1Cache, value))
                    {
                        FinalL1Cache = (uint)Math.Ceiling(m_L1Cache * m_CacheUtilization);
                        TotalCacheSize = FinalL1Cache + FinalL2Cache + FinalL3Cache;
                        PerformOptimizations = IsSettingsAllowOptimizations();
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeSpecsChangedCallback);
                    }
                }
            }

            /// <summary>
            /// L2 Cache size in bytes, specified in advanced performance settings.
            /// </summary>
            public static ulong L2Cache
            {
                get => m_L2Cache;
                set
                {
                    if (Configure.SetValue(L2CacheKey, ref m_L2Cache, value))
                    {
                        FinalL2Cache = (uint)Math.Ceiling(m_L2Cache * m_CacheUtilization);
                        TotalCacheSize = FinalL1Cache + FinalL2Cache + FinalL3Cache;
                        PerformOptimizations = IsSettingsAllowOptimizations();
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeSpecsChangedCallback);
                    }
                }
            }

            /// <summary>
            /// L1 Cache size in bytes, specified in advanced performance settings.
            /// </summary>
            public static ulong L3Cache
            {
                get => m_L3Cache;
                set
                {
                    if (Configure.SetValue(L3CacheKey, ref m_L3Cache, value))
                    {
                        FinalL3Cache = (uint)Math.Ceiling(m_L3Cache * m_CacheUtilization);
                        TotalCacheSize = FinalL1Cache + FinalL2Cache + FinalL3Cache;
                        PerformOptimizations = IsSettingsAllowOptimizations();
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeSpecsChangedCallback);
                    }
                }
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Whether to temporary block scheduling of new callbacks for <see cref="OnCacheSpecsChanged"/>. Resets right before callback is invoked.
            /// </summary>
            /// <remarks>
            /// Should be set to <c>true</c> initially to avoid callbacks during engine initialization.
            /// </remarks>
            private static bool m_BlockCallbacks = true;

            /// <remarks>
            /// Should be disabled by default to avoid services preparing for experimental features during Engine startup.
            /// Enabled in static constructor.
            /// </remarks>
            private static bool m_UseCacheOptimizations = false;

            /// <summary>
            /// [0.0 : 1.0], percent value.
            /// </summary>
            private static float m_CacheUtilization;

            /// <summary>
            /// L1 cache size (in bytes).
            /// </summary>
            private static ulong m_L1Cache;

            /// <summary>
            /// L2 cache size (in bytes).
            /// </summary>
            private static ulong m_L2Cache;

            /// <summary>
            /// L3 cache size (in bytes).
            /// </summary>
            private static ulong m_L3Cache;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Constructors
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            [AfterServiceInitialized(typeof(DefaultConfigurationService))]
            internal static void Initialize()
            {
                Configure.GetValue(UseCacheOptimizationsKey, out m_UseCacheOptimizations, true);
                Configure.GetValue(L1CacheKey, out m_L1Cache);
                Configure.GetValue(L2CacheKey, out m_L2Cache);
                Configure.GetValue(L3CacheKey, out m_L3Cache);
                Configure.GetValue(CacheUtilizationKey, out m_CacheUtilization, def: 0.75f);
                FinalL1Cache = (uint)Math.Ceiling(m_L1Cache * m_CacheUtilization);
                FinalL2Cache = (uint)Math.Ceiling(m_L2Cache * m_CacheUtilization);
                FinalL3Cache = (uint)Math.Ceiling(m_L3Cache * m_CacheUtilization);
                TotalCacheSize = FinalL1Cache + FinalL2Cache + FinalL3Cache;
                PerformOptimizations = IsSettingsAllowOptimizations();
                InvokeSpecsChangedCallback();
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            static bool IsSettingsAllowOptimizations()
                => UseCacheOptimizations
                && FinalL1Cache >= LowerL1CacheBound
                && FinalL2Cache >= LowerL2CacheBound
                && FinalL3Cache >= LowerL3CacheBound;
            static void InvokeSpecsChangedCallback()
            {
                m_BlockCallbacks = false;
                OnCacheSpecsChanged?.Invoke();
            }
        }

        /// <summary>
        /// General information and settings about disks in active use.
        /// <para>
        /// Note: only access this section after <see cref="DefaultConfigurationService"/> has initialized.
        /// </para>
        /// </summary>
        /// <remarks>
        /// Optimizing disk usage might be tough, so best of luck to you here.
        /// However, avoiding any disk writing or doing it asynchronously is usually better way to optimize disk usage.
        /// So be mindful about how you use data in this section.
        /// </remarks>
        public static class Disk
        {
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                 Constants
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Key used to serialize data of <see cref="ReduceDiskWear"/> property.
            /// </summary>
            public const string ReduceDiskWearKey = "@" + nameof(ReduceDiskWear);

            /// <summary>
            /// Key used to serialize data of <see cref="UseDiskSpeedOptimizations"/> property.
            /// </summary>
            public const string UseDiskSpeedOptimizationsKey = "@" + nameof(UseDiskSpeedOptimizations);

            /// <summary>
            /// Key used to serialize data of <see cref="DiskUtilization"/> property.
            /// </summary>
            public const string DiskUtilizationKey = "@" + nameof(DiskUtilization);

            /// <summary>
            /// Key used to serialize data of <see cref="DataDiskReadingSpeed"/> property.
            /// </summary>
            public static readonly string DataDiskReadingSpeedKey;

            /// <summary>
            /// Key used to serialize data of <see cref="DataDiskWritingSpeed"/> property.
            /// </summary>
            public static readonly string DataDiskWritingSpeedKey;

            /// <summary>
            /// Key used to serialize data of <see cref="GameDiskReadingSpeed"/> property.
            /// </summary>
            public static readonly string GameDiskReadingSpeedKey;

            /// <summary>
            /// Key used to serialize data of <see cref="GameDiskWritingSpeed"/> property.
            /// </summary>
            public static readonly string GameDiskWritingSpeedKey;

            /// <summary>
            /// Minimal allowed reading/writing speed, in bytes/s.
            /// </summary>
            public const ulong MinimalSpeed = 1024 * 128; // 128KB




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                   Events
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            public static event Action? OnDiskSpecsChanged;




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                              Public Properties
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Wherever supported, will use a more disk-space hungry saving methods, but will cause less writing to a disk overall, making it wear down slower.
            /// </summary>
            /// <remarks>
            /// Real impacts of this option are untested yet.
            /// Anything related to this will be implemented anyway, I think.
            /// </remarks>
            public static bool ReduceDiskWear
            {
                get => m_ReduceDiskWear;
                set
                {
                    if (Configure.SetValue(ReduceDiskWearKey, ref m_ReduceDiskWear, value))
                    {
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeDiskSpecsChanged);
                    }
                }
            }

            /// <summary>
            /// Whether optimizations based on disk speed are allowed.
            /// </summary>
            public static bool UseDiskSpeedOptimizations
            {
                get => m_UseDiskOptimizations;
                set
                {
                    if (Configure.SetValue(UseDiskSpeedOptimizationsKey, ref m_UseDiskOptimizations, value))
                    {
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeDiskSpecsChanged);
                    }
                }
            }

            /// <summary>
            /// Disk utilization percentage compared to the max disk writing/reading speed, in percent [0.0 : 1.0].
            /// </summary>
            public static float DiskUtilization
            {
                get => m_DiskUtilization;
                set
                {
                    value = Math.Clamp(value, 0f, 1f);
                    if (Configure.SetValue(DiskUtilizationKey, ref m_DiskUtilization, value))
                    {
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeDiskSpecsChanged);
                    }
                }
            }

            // TODO: Make this system more adaptable, because we need to be able to specify more disks.
            /// <summary>
            /// Name of the partition where all dynamic data is stored (settings, temp files, etc.)
            /// </summary>
            public static char DataDiskPartition { get; }

            /// <summary>
            /// Reading speed of a data disk (bytes/s).
            /// Data disk is a disk where dynamic game data is stored (settings, temp files, etc.)
            /// </summary>
            public static ulong DataDiskReadingSpeed
            {
                get => m_DataDiskReadingSpeed;
                set
                {
                    if (Configure.SetValue(DataDiskReadingSpeedKey, ref m_DataDiskReadingSpeed, value))
                    {
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeDiskSpecsChanged);
                    }
                }
            }

            /// <summary>
            /// Writing speed of a data disk (bytes/s).
            /// Data disk is a disk where dynamic game data is stored (settings, temp files, etc.)
            /// </summary>
            public static ulong DataDiskWritingSpeed
            {
                get => m_DataDiskWritingSpeed;
                set
                {
                    if (Configure.SetValue(DataDiskWritingSpeedKey, ref m_DataDiskWritingSpeed, value))
                    {
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeDiskSpecsChanged);
                    }
                }
            }

            /// <summary>
            /// Name of the partition where game files are stored (levels, scenes, StreamingAssets, etc.)
            /// </summary>
            public static char GameDiskPartition { get; }

            /// <summary>
            /// Reading speed of a game disk (bytes/s).
            /// Game disk is a disk where game files are stored (levels, scenes, StreamingAssets, etc.)
            /// </summary>
            public static ulong GameDiskReadingSpeed
            {
                get => m_GameDiskReadingSpeed;
                set
                {
                    if (Configure.SetValue(GameDiskReadingSpeedKey, ref m_GameDiskReadingSpeed, value))
                    {
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeDiskSpecsChanged);
                    }
                }
            }

            /// <summary>
            /// Writing speed of a game disk (bytes/s).
            /// Game disk is a disk where game files are stored (levels, scenes, StreamingAssets, etc.)
            /// </summary>
            public static ulong GameDiskWritingSpeed
            {
                get => m_GameDiskWritingSpeed;
                set
                {
                    if (Configure.SetValue(GameDiskWritingSpeedKey, ref m_GameDiskWritingSpeed, value))
                    {
                        DefaultConfigurationService.Delay(ref m_BlockCallbacks, InvokeDiskSpecsChanged);
                    }
                }
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Fields
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            /// <summary>
            /// Whether to temporary block scheduling of new callbacks for <see cref="OnDiskSpecsChanged"/>. Resets right before callback is invoked.
            /// </summary>
            /// <remarks>
            /// Should be set to <c>true</c> initially to avoid callbacks during engine initialization.
            /// </remarks>
            private static bool m_BlockCallbacks = true;
            private static bool m_ReduceDiskWear;

            /// <remarks>
            /// Should be set to <c>false</c> initially to avoid services initializing with experimental optimization settings.
            /// </remarks>
            private static bool m_UseDiskOptimizations = false;
            private static float m_DiskUtilization = 1.0f; // [0.0 : 1.0], percent value.
            private static ulong m_DataDiskReadingSpeed; // In bytes/s
            private static ulong m_DataDiskWritingSpeed; // In bytes/s
            private static ulong m_GameDiskReadingSpeed; // In bytes/s
            private static ulong m_GameDiskWritingSpeed; // In bytes/s




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                                Constructors
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            static Disk()
            {
                // TODO: Replace with compiler arguments instead.
                // Determines which setting keys should be used for the parameters.
                if (IsDesktop)
                {
                    DataDiskPartition = GetPartitionName(AppContext.BaseDirectory);
                    DataDiskReadingSpeedKey = string.Concat("@", DataDiskPartition, "|ReadingSpeed");
                    DataDiskWritingSpeedKey = string.Concat("@", DataDiskPartition, "|WritingSpeed");

                    GameDiskPartition = GetPartitionName(DefaultConfigurationService.ConfigurationPath);
                    GameDiskReadingSpeedKey = string.Concat("@", GameDiskPartition, "|ReadingSpeed");
                    GameDiskWritingSpeedKey = string.Concat("@", GameDiskPartition, "|WritingSpeed");
                }
                else
                {
                    // Handheld devices and consoles usually write to the same disk.
                    const string DiskReadingSpeedKey = "@DiskReadingSpeed";
                    const string DiskWritingSpeedKey = "@DiskWritingSpeed";
                    DataDiskReadingSpeedKey = DiskReadingSpeedKey;
                    DataDiskWritingSpeedKey = DiskWritingSpeedKey;
                    GameDiskReadingSpeedKey = DiskReadingSpeedKey;
                    GameDiskWritingSpeedKey = DiskWritingSpeedKey;
                    DataDiskPartition = default;
                    GameDiskPartition = default;
                }
            }

            [AfterServiceInitialized(typeof(DefaultConfigurationService))]
            internal static void Initialized()
            {
                Configure.GetValue(ReduceDiskWearKey, out m_ReduceDiskWear, false);
                Configure.GetValue(UseDiskSpeedOptimizationsKey, out m_UseDiskOptimizations, true);
                Configure.GetValue(DiskUtilizationKey, out m_DiskUtilization, 1.0f);
                Configure.GetValue(DataDiskReadingSpeedKey, out m_DataDiskReadingSpeed, 0);
                Configure.GetValue(DataDiskWritingSpeedKey, out m_DataDiskWritingSpeed, 0);
                Configure.GetValue(GameDiskReadingSpeedKey, out m_GameDiskReadingSpeed, 0);
                Configure.GetValue(GameDiskWritingSpeedKey, out m_GameDiskWritingSpeed, 0);
                InvokeDiskSpecsChanged();
            }




            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
            /// .
            /// .                                               Private Methods
            /// .
            /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
            static void InvokeDiskSpecsChanged()
            {
                m_BlockCallbacks = false;
                OnDiskSpecsChanged?.Invoke();
            }

            static char GetPartitionName(string path, char def = 'C')
            {
                try
                {
                    string root = Path.GetPathRoot(path);
                    if (string.IsNullOrWhiteSpace(root)) return GetDefault();

                    char result = char.ToUpper(root[0]);
                    if (result >= 'A' && result <= 'Z') return result;
                    else return GetDefault();
                }
                catch (Exception ex)
                {
                    // TODO: Remove after debugging.
                    EclipseLogger.LogError(ex);
                    EclipseLogger.LogWarning($"Cannot retrieve partition name for path: ({path}). Default value will be used instead.");
                }

                return GetDefault();

                // Simplifications:
                char GetDefault()
                {
                    def = char.ToUpper(def);
                    if (def < 'A' || def > 'Z') def = 'C';
                    return def;
                }
            }
        }
    }
}
