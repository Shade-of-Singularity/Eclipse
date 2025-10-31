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

using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Eclipse.Configuration
{
    /// <summary>
    /// Class holder for your <see cref="GameState"/>.
    /// </summary>
    /// <typeparam name="T">Type of your <see cref="GameState"/> class.</typeparam>
    public static class GameState<T> where T : GameState, new()
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static T Instance
        {
            get
            {
                if (m_Instance is null)
                {
                    m_Instance = EngineService<ConfigurationService>.Instance.Load<T>();
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
        private static T? m_Instance;
    }

    /// <summary>
    /// Custom class for your settings, which doesn't use <see cref="Parameters.Parameter"/>.
    /// </summary>
    /// <remarks>
    /// Works similarly to Naninovel, if you familiar with it.
    /// Not used at the moment - implemented in case there will be a good way to use it.
    /// <para>
    /// Made as a proof of concept.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class GameState
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Retrieves custom <see cref="GameState"/> class from current <see cref="ConfigurationService"/>.
        /// </summary>
        /// <remarks>
        /// Use generic implementation directly instead, if possible.
        /// (e.g. <see cref="GameState{T}.Instance"/>)
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T Get<T>() where T : GameState, new() => GameState<T>.Instance;

        /// <summary>
        /// Serializes this game state.
        /// </summary>
        /// <returns>Serialized data about GameState.</returns>
        public virtual string Serialize() => JsonUtility.ToJson(this);

        /// <summary>
        /// Deserializes given data back into a GameState.
        /// </summary>
        /// <param name="raw">Raw data to deserialize.</param>
        public virtual void Deserialize(string raw) => JsonUtility.FromJsonOverwrite(raw, this);
    }
}
