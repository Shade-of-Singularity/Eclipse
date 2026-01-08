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
using UnityEngine;

namespace Eclipse
{
    /// <summary>
    /// Provides fast, type-safe access to engine-level services registered during initialization.
    /// <para>
    /// Can be used in <see cref="EngineService.Initialize"/> method,
    /// but accessing services before their <see cref="ServiceAttribute.InitializationOrder"/> happens will throw.
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
        public static readonly T Instance = Engine.GetOrThrow<T>();
    }

    /// <summary>
    /// An Eclipse service to be initialized.
    /// </summary>
    /// <remarks>
    /// Add an <see cref="ServiceAttribute"/> to your service class to make it a valid service.
    /// </remarks>
    public abstract class EngineService : IEngineServiceDirectAccess
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Delegates:

        // Events:

        // Properties:
        /// <summary>
        /// Whether service was initialized by engine or not.
        /// </summary>
        /// <remarks>
        /// Set to <c>true</c> *after* initialization. Similarly, set to <c>false</c> *after* unloading.
        /// <para>Status will be set regardless if there was exception during service initialization.</para>
        /// </remarks>
        public bool Initialized { get; protected set; }





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Initializes Engine service.
        /// <para>
        /// Unlike any <see cref="EngineService"/> .ctor (constructor), this method is thread-safe.
        /// (as long as <see cref="ServiceAttribute.ThreadExecutionOrder"/> is <see cref="ServiceAttribute.ThreadExecutionMode.MainThread"/>)
        /// </para>
        /// </summary>
        /// <remarks>
        /// Note: Service won't be even instantiated if you don't have <see cref="ServiceAttribute"/> on your class.
        /// <para>Use <see cref="ServiceAttribute.InitializationOrder"/> to specify initialization order.</para>
        /// </remarks>
        protected abstract void Initialize();

        /// <summary>
        /// Called when <see cref="Engine"/> unloads all the code and resources from the memory.
        /// You are meant to save/serialize the state of your service when this event occurs.
        /// </summary>
        protected abstract void Unload();




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                             Internal Callbacks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        void IEngineServiceDirectAccess.EngineInvokeInitialization()
        {
            if (!Initialized)
            {
                try
                {
                    Initialize();
                }
                catch (Exception ex)
                {
                    Debug.LogException(new Exception($"Failed to initialize {GetType().Name} service!", ex));
                }

                Initialized = true;
            }
        }

        void IEngineServiceDirectAccess.EngineInvokeUnloading()
        {
            if (Initialized)
            {
                try
                {
                    Unload();
                }
                catch (Exception ex)
                {
                    Debug.LogException(new Exception($"Failed to unload {GetType().Name} service!", ex));
                }

                Initialized = false;
            }
        }
    }

    /// <summary>
    /// Interface for directly fire internal engine callbacks.
    /// </summary>
    public interface IEngineServiceDirectAccess
    {
        /// <summary>
        /// Called when <see cref="Engine"/> initialized this service.
        /// </summary>
        void EngineInvokeInitialization();

        /// <summary>
        /// Called when <see cref="Engine"/> unloads this service.
        /// </summary>
        void EngineInvokeUnloading();
    }
}
