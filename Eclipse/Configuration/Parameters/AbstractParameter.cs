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

using Eclipse.Structs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Configuration.Parameters
{
    /// <summary>
    /// Parameter for the settings of the game.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public abstract class AbstractParameter<TValue> : Parameter where TValue : IEquatable<TValue>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public delegate void ValueChangeHandler(TValue old, TValue current);
        public delegate void ModifiedStateChangeHandler(bool modified);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                   Events
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Called when value have changed and was applied.
        /// </summary>
        public event ValueChangeHandler? OnValueChanged;

        /// <summary>
        /// Called when value was applied. Should be used by more expensive systems.
        /// </summary>
        public event ValueChangeHandler? OnValueApplied;

        /// <summary>
        /// Called when <see cref="IsModified"/> has changed.
        /// </summary>
        public event ModifiedStateChangeHandler? OnModifiedChanged;


        // Fire-on-add events:
        // TODO: Auto-fire regular events when Engine was initialized.
        /// <summary><inheritdoc cref="OnValueChanged"/></summary>
        /// <remarks>
        /// Both attaches the event handler to <see cref="OnValueChanged"/>, and instantly fires the event for it.
        /// </remarks>
        public event ValueChangeHandler FireWithValueChanged
        {
            remove => OnValueChanged -= value;
            add
            {
                if (value != null)
                {
                    OnValueApplied += value;
                    value(m_Value, m_Value);
                }
            }
        }

        /// <summary><inheritdoc cref="OnValueApplied"/></summary>
        /// <remarks>
        /// Both attaches the event handler to <see cref="OnValueApplied"/>, and instantly fires the event for it.
        /// </remarks>
        public event ValueChangeHandler FireWithValueApplied
        {
            remove => OnValueApplied -= value;
            add
            {
                if (value != null)
                {
                    OnValueApplied += value;
                    value(m_LastValue, m_LastValue);
                }
            }
        }

        /// <summary><inheritdoc cref="OnModifiedChanged"/></summary>
        /// <remarks>
        /// Both attaches the event handler to <see cref="OnValueApplied"/>, and instantly fires the event for it.
        /// </remarks>
        public event ModifiedStateChangeHandler FireWithModifiedChanged
        {
            remove => OnModifiedChanged -= value;
            add
            {
                if (value != null)
                {
                    OnModifiedChanged += value;
                    value(IsModified);
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public bool IsDirty => !EqualityComparer<TValue>.Default.Equals(m_LastValue, m_Value);
        public bool IsModified => !EqualityComparer<TValue>.Default.Equals(m_Value, m_DefaultValue);

        /// <summary>
        /// Default value of the parameter to be used.
        /// </summary>
        /// <remarks>
        /// If <see cref="Value"/> equals to current <see cref="DefaultValue"/> when setting it
        /// (e.g. when not <see cref="IsModified"/>) - will set <see cref="Value"/> to a new <see cref="DefaultValue"/>.
        /// <para>
        /// Because of that, it is recommended to not modify this value outside of the initialization, to not interfere with user choices.
        /// </para>
        /// </remarks>
        public TValue DefaultValue
        {
            get => m_DefaultValue;
            set => SetDefault(value);
        }

        /// <summary>
        /// Value, stored in the configuration file.
        /// </summary>
        /// <remarks>
        /// Set value can be reverted with <see cref="ConfigurationService.Revert"/>, if anything was changed.
        /// </remarks>
        public TValue Value
        {
            get => m_Value;
            set => Set(value);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Fields
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        // Static Fields:

        // Encapsulated Fields:
        protected TValue m_DefaultValue;
        protected TValue m_Value;

        // Local Fields:
        protected TValue m_LastValue;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public AbstractParameter(FullName name) : this(name, default!) { }
        public AbstractParameter(FullName name, TValue def) : base(name)
        {
            // TODO: Actually load-in the values from storage XD
            m_Value = m_LastValue = m_DefaultValue = def;
            EngineService<ConfigurationService>.Instance.OnAfterApplyChanges += ApplyChanges;
            EngineService<ConfigurationService>.Instance.OnAfterRevertChanges += RevertChanges;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override string Serialize() => JsonUtility.ToJson(Value);
        public override void Deserialize(string raw) => Value = JsonUtility.FromJson<TValue>(raw);
        public override void ApplyChanges()
        {
            if (IsDirty) ApplyChangesForceFireCallbacks();
        }

        public override void ApplyChangesForceFireCallbacks()
        {
            TValue old = m_LastValue;
            m_LastValue = m_Value;
            OnValueApplied?.Invoke(old, m_Value);
        }

        public override void RevertChanges()
        {
            if (IsDirty)
            {
                RevertChangesForceFireCallbacks();
            }
        }

        public override void RevertChangesForceFireCallbacks()
        {
            bool modified = IsModified;
            TValue old = m_Value;
            m_Value = m_LastValue;
            OnValueApplied?.Invoke(old, m_LastValue);
            OnValueChanged?.Invoke(old, m_LastValue);
            if (IsModified != modified)
            {
                OnModifiedChanged?.Invoke(!modified);
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public virtual void Set(TValue value)
        {
            if (!EqualityComparer<TValue>.Default.Equals(m_Value, value))
            {
                bool modified = IsModified;
                TValue old = m_Value;
                m_Value = value;
                OnValueChanged?.Invoke(old, value);
                if (IsModified != modified) OnModifiedChanged?.Invoke(!modified);
            }
        }

        private void SetDefault(TValue value)
        {
            if (!EqualityComparer<TValue>.Default.Equals(m_DefaultValue, value))
            {
                // Also updates current value to the new one.
                if (!IsModified)
                {
                    m_DefaultValue = value;
                    Set(value);
                }
                else
                {
                    m_DefaultValue = value;
                }
            }
        }

        /// <summary>
        /// Resets <see cref="Value"/> to a <see cref="DefaultValue"/>.
        /// </summary>
        public void Reset() => Set(DefaultValue);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
    }
}
