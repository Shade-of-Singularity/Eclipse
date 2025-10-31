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

namespace Eclipse.Configuration
{
    /// <summary>
    /// Reconfiguration service for you to target with <see cref="ServicePreloadMethodAttribute"/>s or <see cref="ServiceAfterloadMethodAttribute"/>s
    /// in order to reconfigure <see cref="EngineConfiguration{T}"/>s.
    /// </summary>
    /// <remarks>
    /// Cannot be inherited as it should not be replaced by any other service.
    /// </remarks>
    [Service(InitializationOrder = InitializationOrder, ThreadExecutionOrder = ServiceAttribute.ThreadExecutionMode.MainThread)]
    public sealed class ReconfigurationService : EngineService
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const int InitializationOrder = ConfigurationService.InitializationOrder + 1000;
        /// <see cref="ReconfigurationService"/> itself doesn't reconfigure anything.
        /// Code example: <![CDATA[
        /// [ServicePreloadMethod(typeof(ReconfigurationService), InvokeOrder = 0, ThreadSafe = false)]
        /// public static void AllowAutoSave()
        /// {
        ///     EngineConfiguration<ConfigurationSettings>.Instance.SettingsAutoSave = true;
        ///     EngineConfiguration<ConfigurationSettings>.Instance.SettingsAutoSaveDelay = 10f;
        /// }
        /// ]]>
    }
}
