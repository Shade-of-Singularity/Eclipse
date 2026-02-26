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

namespace ServiceCore.Serialization
{
    /// <summary>
    /// Serialization service which adjusts how serialization happens in the entire Engine.
    /// Methods for serialization can be retrieved via <see cref="Serializers"/> and <see cref="Serializers{TValue}"/> classes.
    /// </summary>
    /// <remarks>
    /// <see cref="ISerializationService"/> adds default serialization methods to the list,
    /// so use <see cref="AfterServiceInitializedAttribute"/> to add your own methods after default ones are provided.
    /// </remarks>
    public interface ISerializationService : IService<ISerializationService>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Constants
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// One of the first services in the game to be initialized, as core things, like ConfigurationsServices, is meant to depend on it.
        /// </summary>
        public const int InitializationOrder = -1_800_000_000;
        /// <summary>
        /// Prefix for messages sent to the console from this class.
        /// </summary>
        public const string LogPrefix = Engine.LogPrefix + "[SerializationService]";
    }
}
