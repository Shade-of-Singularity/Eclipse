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

namespace ServiceCore
{
    /// <summary>
    /// Initializes attached <see cref="MonoService{T}"/> instances manually at startup, in a given order.
    /// </summary>
    [DefaultExecutionOrder(0)]
    public class ServiceInitializer : MonoBehaviour
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
        /// <see cref="ITerminationArgs"/> to use in <see cref="IService.InvokeInitialize(IInitializationArgs)"/>.
        /// Defaults to current <see cref="Engine.State"/> if not provided.
        /// </summary>
        public IInitializationArgs? InitializationArgs { get; set; }

        /// <summary>
        /// <see cref="ITerminationArgs"/> to use in <see cref="IService.InvokeTerminate(ITerminationArgs)"/>.
        /// Defaults to current <see cref="Engine.State"/> if not provided.
        /// </summary>
        public ITerminationArgs? TerminationArgs { get; set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:
        private static ServiceInitializer? m_Instance;

        // Serialized Fields:
        [SerializeField] private readonly IService[]? m_Services;

        // Encapsulated Fields:

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Unity Callbacks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private async void Awake()
        {
            if (m_Instance)
            {
                Debug.LogWarning($"{Engine.LogPrefix} Only one Instance of {nameof(ServiceInitializer)} is permitted to exist at once. Newer Instance will be destroyed.");
                Destroy(gameObject);
                return;
            }

            m_Instance = null;
            // TODO: Halt regular engine initialization (or force it to wait) until MonoServices are initialized.
            //  Alternatively, we can insert mono services in the initialization loop via dependencies and service ordering.
            var array = m_Services;
            if (array is null) return;
            var args = InitializationArgs ?? Engine.State;
            for (int i = 0; i < array.Length; i++)
            {
                await array[i].InvokeInitialize(args);
            }
        }

        private async void Destroy()
        {
            if (m_Instance != this)
            {
                return;
            }

            // TODO: Unity won't wait for Destroy to finish. We need to schedule destruction using Engine itself, or using QuitHandler.
            var array = m_Services;
            if (array is null) return;
            var args = TerminationArgs ?? Engine.State;
            for (int i = 0; i < array.Length; i++)
            {
                await array[i].InvokeTerminate(args);
            }

            m_Instance = null;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

    }
}
