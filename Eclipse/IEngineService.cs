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
    /// An StackControl service to be initialized.
    /// </summary>
    /// <remarks>
    /// Add an <see cref="ServiceAttribute"/> to your service class to make it a valid service.
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
        protected virtual void Initialize() { }

        /// <summary>
        /// Called when <see cref="Engine"/> unloads all the code and resources from the memory.
        /// You are meant to save/serialize the state of your service when this event occurs.
        /// </summary>
        protected virtual void Unload() { }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                             Internal Callbacks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        void IEngineServiceDirectControlHandler.EngineInvokeInitialization()
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

        void IEngineServiceDirectControlHandler.EngineInvokeUnloading()
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
    public interface IEngineServiceDirectControlHandler
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
