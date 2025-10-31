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

using System.Runtime.CompilerServices;
using UnityEngine;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Provides fast, type-safe access to engine-level configurations registered during initialization.
    /// <para>
    /// Can be used in <see cref="EngineService.Initialize"/> method after <see cref="ConfigurationService"/> was initialized.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Will return default <see cref="EngineConfiguration"/> if it wasn't loaded or found in a safe-file.
    /// </remarks>
    /// <typeparam name="T">The type of the configuration to retrieve. Must inherit from <see cref="EngineConfiguration"/>.</typeparam>
    public static class EngineConfiguration<T> where T : EngineConfiguration
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Cached instance of the requested configuration class.
        /// </summary>
        public static T Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    m_Instance = EngineService<ConfigurationService>.Instance.GetOrNew<T>();
                    Engine.OnEngineResetting += () => m_Instance = null;
                }

                return m_Instance;
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// As much as I hate to make code less optimized, <c>static readonly T Instance</c>
        /// was replaced with parameter to allow configuration modifications (not supported yet (TODO: this)).
        /// </summary>
        /// <remarks>
        /// As much as I want to simply use a readonly field with static initializer here,
        /// we need a null check to support full runtime engine reloading and reference modification.
        /// Note: If we will ever have engine that does not support runtime reloading - use static readonly fields instead.
        /// </remarks>
        private static T? m_Instance;
    }

    /// <summary>
    /// Base class for configuration files for internal systems and services, so they can be configured via Unity.
    /// </summary>
    public abstract class EngineConfiguration : ScriptableObject
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves <see cref="EngineConfiguration"/> class.
        /// </summary>
        /// <remarks>
        /// Use generic implementation directly instead, if possible.
        /// (e.g. <see cref="EngineConfiguration{T}.Instance"/>)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>() where T : EngineConfiguration => EngineConfiguration<T>.Instance;
    }
}
