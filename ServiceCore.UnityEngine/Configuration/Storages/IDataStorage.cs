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

using ServiceCore.Parameters;

namespace ServiceCore.Configuration.Storages
{
    public abstract class DataStorage<T> : IDataStorage where T : DataStorage<T>, new()
    {
        /// <summary>
        /// Static m_Instance of an <see cref="IDataStorage"/> implementation.
        /// </summary>
        public static readonly T Instance = new();

        /// <inheritdoc cref="IDataStorage.Load(AbstractParameter)"/>
        public abstract void Load(AbstractParameter parameter);

        /// <inheritdoc cref="IDataStorage.Save(AbstractParameter)"/>
        public abstract void Save(AbstractParameter parameter);
    }

    /// <summary>
    /// Processor interface which controls where data is stored.
    /// </summary>
    public interface IDataStorage
    {
        /// <summary>
        /// If has any data about it - will call <see cref="AbstractParameter.Deserialize(string)"/> using a raw string data about this parameter.
        /// </summary>
        /// <param Identifier="parameter">Parameter to be updated.</param>
        public void Load(AbstractParameter parameter);

        /// <summary>
        /// Calls <see cref="AbstractParameter.Serialize"/> method and stores a return value 
        /// </summary>
        /// <param Identifier="parameter"></param>
        public void Save(AbstractParameter parameter);
    }
}
