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

using Eclipse.Configuration.Parameters;

namespace Eclipse.Configuration.Storages
{
    public abstract class ParameterStorage<T> : IParameterStorage where T : ParameterStorage<T>, new()
    {
        /// <summary>
        /// Static instance of an <see cref="IParameterStorage"/> implementation.
        /// </summary>
        public static readonly T Instance = new();

        /// <inheritdoc cref="IParameterStorage.Load(Parameter)"/>
        public abstract void Load(Parameter parameter);

        /// <inheritdoc cref="IParameterStorage.Save(Parameter)"/>
        public abstract void Save(Parameter parameter);
    }

    /// <summary>
    /// Processor interface which controls where data is stored.
    /// </summary>
    public interface IParameterStorage
    {
        /// <summary>
        /// If has any data about it - will call <see cref="Parameter.Deserialize(string)"/> using a raw string data about this parameter.
        /// </summary>
        /// <param name="parameter">Parameter to be updated.</param>
        public void Load(Parameter parameter);

        /// <summary>
        /// Calls <see cref="Parameter.Serialize"/> method and stores a return value 
        /// </summary>
        /// <param name="parameter"></param>
        public void Save(Parameter parameter);
    }
}
