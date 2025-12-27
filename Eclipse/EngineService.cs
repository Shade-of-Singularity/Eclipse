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

namespace Eclipse
{
    /// <summary>
    /// Provides fast, type-safe access to engine-level services registered during initialization.
    /// <para>
    /// Can be used in <see cref="EngineService.Initialize"/> method,
    /// but service might not be initialized depending on <see cref="ServiceAttribute.InitializationOrder"/>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// Will throw if requested <see cref="EngineService"/> was not created on initialization.
    /// Use '<see cref="Engine.TryGet{T}(out T)"/>' or '<see cref="Engine.GetOrDefault{T}(T)"/>' if you need to handle missing services more gracefully.
    /// </remarks>
    /// <typeparam name="T">The type of the service to retrieve. Must inherit from <see cref="EngineService"/>.</typeparam>
    public static class EngineService<T> where T : EngineService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Cached instance of the requested service. Throws if the service was not registered.
        /// </summary>
        /// <remarks>
        /// Use '<see cref="Engine.TryGet{T}(out T)"/>' or '<see cref="Engine.GetOrDefault{T}(T)"/>' if you need to handle missing services more gracefully.
        /// </remarks>
        /// Note: Do NOT replace with <see cref="Engine.GetOrDefault{T}(T)"/>!
        /// This is a readonly field! It won't update after first <see cref="Engine.GetOrDefault{T}(T)"/> usage!
        public static T Instance
        {
            get
            {
                if (m_Instance is null)
                {
                    m_Instance = Engine.GetOrThrow<T>();
                    Engine.OnEngineResetting += () => m_Instance = default!;
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
        /// Internal instance holder field.
        /// </summary>
        /// <remarks>
        /// It would have been far more optimized to use direct field reference instead without null check...
        /// But we need null checks to provide reliable Engine reloading at runtime.
        /// </remarks>
        private static T m_Instance = default!;
    }
}
