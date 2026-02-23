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
using UnityEngine;

namespace ServiceCore.Configuration.UI
{
    /// <summary>
    /// UI component for an <see cref="AbstractParameter"/>.
    /// </summary>
    /// <remarks>
    /// When implementing, don't forget that <see cref="Parameter"/>s can be modified in the editor, on <see cref="Application.isPlaying"/> == <c>false</c>.
    /// Account for that by not modifying the UI in such cases, but you can modify the parameters themselves.
    /// </remarks>
    /// <typeparam name="TParameter">Special type of the parameter to try to look for. Specify <see cref="AbstractParameter"/> for any type.</typeparam>
    public abstract class ParameterUI<TParameter> : MonoBehaviour where TParameter : AbstractParameter
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Events:

        // Properties:
        public TParameter? Parameter { get; protected set; }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Serialized Fields:
        [Header("Settings")]
        [Tooltip("Name of the parameter to process.")]
        [SerializeField] private string m_ParameterID = string.Empty;
        [Tooltip("Whether to set value of the UI as a default value of the parameter.")]
        [SerializeField] private bool m_SetAsDefault = true;

        // Encapsulated Fields:

        // Local Fields:





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Unity Callbacks
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected virtual void Awake()
        {
            SetupParameter(ParameterManager.Get<TParameter>(m_ParameterID));
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>





        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Protected Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        protected virtual void SetupParameter(TParameter parameter)
        {
            Parameter = parameter;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

    }
}
