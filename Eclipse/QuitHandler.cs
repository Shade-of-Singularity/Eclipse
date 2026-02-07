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

using UnityEngine;

namespace Eclipse
{
    /// <inheritdoc cref="QuitHandler"/>
    /// <remarks>
    /// Also provides static field with a default instance of the unloader.
    /// </remarks>
    public abstract class QuitHandler<T> : QuitHandler where T : QuitHandler<T>, new()
    {
        /// <summary>
        /// Default instance of the handler to be used.
        /// </summary>
        public static readonly T Instance = new T();
    }

    /// <summary>
    /// Manages <see cref="Engine"/> automatic and graceful unloading on <see cref="Application.wantsToQuit"/>.
    /// </summary>
    /// <seealso cref="DefaultQuitHandler"/>
    public abstract class QuitHandler
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Current unloader instance. Will use <see cref="DefaultQuitHandler"/> by default.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Initialize"/> to replace it with your own.
        /// </remarks>
        public static QuitHandler Current
        {
            get => m_Current;
            private set
            {
                value ??= DefaultQuitHandler.Instance;
                lock (_lock)
                {
                    if (m_Current == value) return;
                    if (Enabled)
                    {
                        m_Current?.OnDisabled();
                        m_Current = value;
                        value.OnEnabled();
                    }
                    else
                    {
                        m_Current = value;
                    }
                }
            }
        }

        /// <summary>
        /// Whether this <see cref="QuitHandler"/> actively prevents engine unloading or not at the moment.
        /// </summary>
        /// <remarks>
        /// Enabled right before <see cref="Engine"/> starts initialization, and disabled the moment initialization ends.
        /// </remarks>
        public static bool Enabled
        {
            get => m_Enabled;
            private set
            {
                lock (_lock)
                {
                    if (m_Enabled == value) return;
                    if (m_Enabled = value)
                    {
                        m_Current?.OnEnabled();
                    }
                    else
                    {
                        m_Current?.OnDisabled();
                    }
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private static readonly object _lock = new object();
        private static QuitHandler m_Current = DefaultQuitHandler.Instance; // Will use default unloader by default.
        private static volatile bool m_Enabled = false; // Enabled by Eclipse.Engine callback during initialization.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [EclipseInitialize(InitializationTiming.BeforeEngineInitialization)]
        internal static void PreventQuitting() => Enabled = true;

        [EclipseTermination(TerminationTiming.AfterEngineTermination)]
        internal static void AllowQuitting() => Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attaches this <see cref="QuitHandler"/> instance as active instance in <see cref="Current"/> property.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If <see cref="Enabled"/> is <c>true</c> - will also immediately run <see cref="OnEnabled"/> method.
        /// </para>
        /// <para>
        /// Will also run <see cref="OnDisabled"/> on instance that was attached to <see cref="Current"/> before.
        /// </para>
        /// </remarks>
        public virtual void Initialize() => Current = this;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                  Abstract
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Quit handler which prevents <see cref="Application.wantsToQuit"/> callback.
        /// </summary>
        /// <remarks>
        /// Should return <c>false</c>, indicating that you do NOT allow application to quit just yet.
        /// Should return <c>true</c> otherwise, as indication that you allow application to quit.
        /// </remarks>
        protected abstract bool Interrupt();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Runs whenever unloader should be enabled and get ready for ensuring graceful engine unloading.
        /// </summary>
        protected virtual void OnEnabled()
        {
            lock (_lock)
            {
                if (!m_Enabled)
                {
                    return;
                }

                Application.wantsToQuit -= Interrupt;
                m_Enabled = false;
            }
        }

        /// <summary>
        /// Runs whenever unloader is disabled by any means.
        /// </summary>
        protected virtual void OnDisabled()
        {
            lock (_lock)
            {
                if (m_Enabled)
                {
                    return;
                }

                Application.wantsToQuit += Interrupt;
                m_Enabled = true;
            }
        }
    }
}
