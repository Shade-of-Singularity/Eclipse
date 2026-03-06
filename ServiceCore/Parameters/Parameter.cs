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

using ServiceCore.Serialization;
using System;
using System.Collections.Generic;

namespace ServiceCore.Parameters
{
    /// <summary>
    /// Parameter for the settings of the game.
    /// </summary>
    /// <remarks>
    /// Rule of thumbs as to when to use those parameters:
    /// <para>- if parameter is modifiable at runtime - use <see cref="Parameter{TValue}"/>.</para>
    /// <para>- if parameter can be modified by other mods at runtime - use <see cref="Parameter{TValue}"/>.</para>
    /// <para>- if parameter is only set at launch or engine restart - use C# Properties.</para>
    /// <para>- if parameter can be 'set' at runtime, but value will only update after reload - use <see cref="SolidParameter{TValue}"/>.</para>
    /// </remarks>
    /// <typeparam Identifier="TValue">Type of the variable parameter stores.</typeparam>
    public sealed class Parameter<TValue> : AbstractParameter where TValue : IEquatable<TValue>
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Handler for when value of the parameter has changed.
        /// </summary>
        /// <remarks>
        /// Can also be used by UI components to check for <see cref="IsDefault"/> and <see cref="IsDirty"/> property states.
        /// </remarks>
        /// <param Identifier="parameter">Parameter that was modified.</param>
        /// <param Identifier="previous">Previous value of the parameter before it was changed.</param>
        public delegate void ValueChangeHandler(Parameter<TValue> parameter, TValue previous);




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

        // Fire-on-add events:
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
                    value(this, m_Value);
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
                    value(this, m_LastValue);
                }
            }
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                              Public Properties
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override bool IsDirty => !EqualityComparer<TValue>.Default.Equals(m_LastValue, m_Value);

        /// <inheritdoc/>
        public override bool IsDefault => EqualityComparer<TValue>.Default.Equals(m_Value, m_DefaultValue);

        /// <summary>
        /// Default value of the parameter to be used.
        /// </summary>
        /// <remarks>
        /// If <see cref="Value"/> equals to current <see cref="DefaultValue"/> when setting it
        /// (e.g. when not <see cref="IsDefault"/>) - will set <see cref="Value"/> to a new <see cref="DefaultValue"/>.
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
        /// Set value can be reverted with <see cref="DefaultConfigurationService.Revert"/>, if anything was changed.
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
        private TValue m_DefaultValue;
        private TValue m_Value;

        // Local Fields:
        private TValue m_LastValue;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Simple constructor for <see cref="Parameter{TValue}"/>.
        /// </summary>
        public Parameter(string id) : this(id, default!) { }

        /// <summary>
        /// Full constructor for <see cref="Parameter{TValue}"/>. Allows specifying <paramref Identifier="def"/>ault value.
        /// </summary>
        public Parameter(string id, TValue def) : base(id)
        {
            // Parameter values are loaded from disk and applied during ConfigurationService initialization.
            m_Value = m_LastValue = m_DefaultValue = def;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Static Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public static Parameter<TValue> Get(string id, TValue def = default!)
        {
            return ParameterManager.GetOrNew(id, def, static (id, def) => new Parameter<TValue>(id, def));
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <inheritdoc/>
        public override object GetValue() => Value;

        /// <inheritdoc/>
        public override string Serialize() => Serializers<TValue>.Serializer(Value);

        /// <inheritdoc/>
        public override void Deserialize(string raw)
        {
            try
            {
                Value = Serializers<TValue>.Deserializer(raw);
            }
            catch (Exception ex)
            {
                ServiceCoreLogger.LogWarning($"Parameter {ID} ({typeof(TValue).Name}]) was not able to deserialize properly. Default value ({m_DefaultValue}) will be used instead.\nFailed data: {raw}\nException: {ex}");
            }
        }

        /// <inheritdoc/>
        public override void ApplyChanges()
        {
            if (IsDirty) ApplyChangesForceFireCallbacks();
        }

        /// <inheritdoc/>
        public override void ApplyChangesForceFireCallbacks()
        {
            TValue old = m_LastValue;
            m_LastValue = m_Value;
            OnValueApplied?.Invoke(this, old);
        }

        /// <inheritdoc/>
        public override void RevertChanges()
        {
            if (IsDirty)
            {
                RevertChangesForceFireCallbacks();
            }
        }

        /// <inheritdoc/>
        public override void RevertChangesForceFireCallbacks()
        {
            TValue old = m_Value;
            m_Value = m_LastValue;
            OnValueApplied?.Invoke(this, old);
            OnValueChanged?.Invoke(this, old);
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Resets <see cref="Value"/> to a <see cref="DefaultValue"/>.
        /// </summary>
        public void Reset() => Set(DefaultValue);
        //public void ForceSet




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void Set(TValue value)
        {
            if (!EqualityComparer<TValue>.Default.Equals(m_Value, value))
            {
                TValue old = m_Value;
                m_Value = value;
                OnValueChanged?.Invoke(this, old);
            }
        }

        private void SetDefault(TValue value)
        {
            if (!EqualityComparer<TValue>.Default.Equals(m_DefaultValue, value))
            {
                // Also updates current value to the new one.
                if (IsDefault)
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
    }
}
