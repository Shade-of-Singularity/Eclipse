using System;
using UnityEngine;

namespace Eclipse
{
    /// <summary>
    /// Default Engine unloader implementation.
    /// </summary>
    /// <remarks>
    /// You can inherit <see cref="EngineUnloader"/> to make an animated exit screen or something like that)
    /// </remarks>
    public sealed class DefaultUnloader : EngineUnloader
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Static Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static readonly DefaultUnloader Instance = new DefaultUnloader();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private bool m_IsUnloading;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected override bool QuitHandler()
        {
            // If already unloads - no reason to start another unloading session.
            if (m_IsUnloading) return false;
            if (Enabled && !m_IsUnloading)
            {
                // Allows starting Engine unloading only 
                AsyncQuit();
            }

            return false;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Actually runs <see cref="Engine.Unload"/>.
        /// </summary>
        /// <remarks>
        /// See <see cref="QuitHandler"/> to understand how it is used.
        /// </remarks>
        private async void AsyncQuit()
        {
            m_IsUnloading = true;
            try
            {
                await Engine.Unload();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            m_IsUnloading = false;
            Application.Quit(0);
        }
    }

    /// <summary>
    /// Manages <see cref="Engine"/> automatic and graceful unloading.
    /// </summary>
    public abstract class EngineUnloader
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Current unloader instance. Will use <see cref="DefaultUnloader"/> by default.
        /// </summary>
        /// <remarks>
        /// Use <see cref="Initialize"/> to replace it with your own.
        /// </remarks>
        public static EngineUnloader Current
        {
            get => m_Current;
            private set
            {
                value ??= DefaultUnloader.Instance;
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
        /// Whether unloaded actively prevents engine unloading or not at the moment.
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
        private static EngineUnloader m_Current = DefaultUnloader.Instance; // Will use default unloader by default.
        private static volatile bool m_Enabled = false; // Enabled by Eclipse.Engine callback during initialization.




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [EclipseInitialize(InitializationTiming.BeforeEngineInitialization)]
        internal static void PreventQuitting() => Enabled = true;

        [EclipseUnloading(UnloadingTiming.AfterEngineUnloading)]
        internal static void AllowQuitting() => Enabled = false;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Attaches this <see cref="EngineUnloader"/> instance as active instance in <see cref="Current"/> property.
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
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Quit handler which prevents <see cref="Application.wantsToQuit"/> callback.
        /// </summary>
        /// <remarks>
        /// Should return <c>false</c>, indicating that you do NOT allow application to quit just yet.
        /// Should return <c>true</c> otherwise, as indication that you allow application to quit.
        /// </remarks>
        protected abstract bool QuitHandler();

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

                Application.wantsToQuit -= QuitHandler;
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

                Application.wantsToQuit += QuitHandler;
                m_Enabled = true;
            }
        }
    }
}
