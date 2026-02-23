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

namespace ServiceCore.Configuration
{
    /// <summary>
    /// Reconfiguration service for you to target with <see cref="BeforeServiceInitializedAttribute"/>s or <see cref="AfterServiceInitializedAttribute"/>s
    /// in order to reconfigure <see cref="EngineConfiguration{T}"/>s.
    /// </summary>
    /// <remarks>
    /// Cannot be inherited as it should not be replaced by any other service.
    /// </remarks>
    [Service(IReconfigurationService.InitializationOrder)]
    public class ReconfigurationService : IReconfigurationService
    {
        /// <see cref="ReconfigurationService"/> itself doesn't reconfigure anything.
        /// Code example: <![CDATA[
        /// [AfterServiceInitialized(typeof(IReconfigurationService), InvokeOrder = 0, ThreadSafe = false)]
        /// public static void AllowAutoSave()
        /// {
        ///     EngineConfiguration<ConfigurationSettings>.Instance.SettingsAutoSave = true;
        ///     EngineConfiguration<ConfigurationSettings>.Instance.SettingsAutoSaveDelay = 10f;
        /// }
        /// ]]>




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public virtual UniTask Initialize() => UniTask.CompletedTask;

        /// <inheritdoc/>
        public virtual UniTask Terminate() => UniTask.CompletedTask;
    }

    /// <summary>
    /// Reconfiguration service for you to target with <see cref="BeforeServiceInitializedAttribute"/>s or <see cref="AfterServiceInitializedAttribute"/>s
    /// in order to reconfigure <see cref="EngineConfiguration{T}"/>s.
    /// </summary>
    /// <remarks>
    /// Cannot be inherited as it should not be replaced by any other service.
    /// </remarks>
    public interface IReconfigurationService : IService<IReconfigurationService>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public const int InitializationOrder = IConfigurationService.InitializationOrder + 1000;
    }
}
