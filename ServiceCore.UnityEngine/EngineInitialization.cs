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

using Cysharp.Threading.Tasks;
using UnityEngine;

namespace ServiceCore
{
    /// <summary>
    /// Main class for <see cref="ServiceCore"/> Foundation Library.
    /// </summary>
    public static class EngineInitialization
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                       Unity Initialization Callbacks
        /// .                                TODO: Add Editor-time initialization methods.
        /// .                                   TODO: Move to ServiceCore.UnityEngine.dll
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnSubsystemRegistration()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.SubsystemRegistration)
            {
                Engine.Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void OnAfterAssembliesLoaded()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.AfterAssembliesLoaded)
            {
                Engine.Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void OnBeforeSplashScreen()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.BeforeSplashScreen)
            {
                Engine.Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void OnBeforeSceneLoad()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.BeforeSceneLoad)
            {
                Engine.Initialize().Forget();
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void OnAfterSceneLoad()
        {
            if (EclipseConfiguration.Instance.InitializationType == AutomaticStartupType.AfterSceneLoad)
            {
                Engine.Initialize().Forget();
            }
        }
    }
}
