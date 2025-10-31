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

namespace Eclipse.Configuration
{
    public class ConfigurationSettings : EngineConfiguration
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
        /// Automatically applies all changes made to <see cref="Parameters.Parameter"/>.
        /// Can be useful if there is only simple settings on UI, and you don't want to bother with "Apply/Revert" much.
        /// </summary>
        public bool SettingsAutoApply
        {
            get => m_SettingsAutoApply;
            set => m_SettingsAutoApply = value;
        }

        /// <summary>
        /// Whether to do auto-save of <see cref="Parameters.Parameter"/>s during the game.
        /// When <c>false</c> - settings will only be saved when exiting the game.
        /// </summary>
        public bool SettingsAutoSave
        {
            get => m_SettingsAutoSave;
            set => m_SettingsAutoSave = value;
        }

        /// <summary>
        /// How long to wait (is seconds) after changes to any <see cref="Parameters.Parameter"/> were applied?
        /// Used to guarantee that settings were saved to the disk in case crash occurs or user exits via Terminal.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        public float SettingsAutoSaveDelay
        {
            get => m_SettingsAutoSaveDelay;
            set => m_SettingsAutoSaveDelay = Mathf.Max(0.5f, value);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:
        [SerializeField] private bool m_SettingsAutoApply = true;
        [SerializeField] private bool m_SettingsAutoSave = true;
        [SerializeField, Min(1f)] private float m_SettingsAutoSaveDelay = 5f;

        // Local Fields:





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




#if UNITY_EDITOR
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Editor
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>

#endif
    }
}
