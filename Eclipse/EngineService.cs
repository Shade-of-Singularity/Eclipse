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
        /// Internal instance holder field.
        /// </summary>
        /// <remarks>
        /// It would have been far more optimized to use direct field reference instead without null check...
        /// But we need null checks to provide reliable Engine reloading at runtime.
        /// </remarks>
        private static T? m_Instance;
    }

    /// <summary>
    /// An StackControl service to be initialized.
    /// </summary>
    /// <remarks>
    /// Add an <see cref="ServiceAttribute"/> to your final service implementation.
    /// </remarks>
    public abstract class EngineService : IEngineServiceDirectControlHandler
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Events:

        // Properties:
        public bool Initialized { get; private set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        void IEngineServiceDirectControlHandler.EngineInvokeInitialization()
        {
            if (!Initialized)
            {
                Initialize();
                Initialized = true;
            }
        }

        void IEngineServiceDirectControlHandler.EngineInvokeUnloading()
        {
            if (Initialized)
            {
                Unload();
                Initialized = false;
            }
        }

        /// <summary>
        /// Initializes Engine service.
        /// <para>
        /// Unlike any <see cref="EngineService"/> .ctor (constructor), this method is thread-safe.
        /// (as long as <see cref="ServiceAttribute.ThreadExecutionOrder"/> is <see cref="ServiceAttribute.ThreadExecutionMode.MainThread"/>)
        /// </para>
        /// </summary>
        /// <remarks>
        /// Won't be even instantiated if you don't have <see cref="ServiceAttribute"/> on your class.
        /// Use <see cref="ServiceAttribute.InitializationOrder"/> to specify initialization order.
        /// </remarks>
        protected virtual void Initialize() { }

        /// <summary>
        /// Called when <see cref="Engine"/> unloads all the code and resources from the memory.
        /// You are meant to save/serialize the state of your service when this event occurs.
        /// </summary>
        protected virtual void Unload() { }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

    }

    public interface IEngineServiceDirectControlHandler
    {
        void EngineInvokeInitialization();
        void EngineInvokeUnloading();
    }
}
