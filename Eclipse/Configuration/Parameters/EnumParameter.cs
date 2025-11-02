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

using Eclipse.Serialization;
using Eclipse.Structs;
using System;
using System.Collections.Generic;

namespace Eclipse.Configuration.Parameters
{
    /// <summary>
    /// Specialized enum parameter handler.
    /// </summary>
    /// <remarks>
    /// TODO: Provide a way to specify localization keys for each enum value.
    /// Maybe with custom attributes for each enum value, and cache the reflection using system similar to <see cref="Engine.Get{T}"/>?
    /// Hopefully it won't overload code database with all those generics, though I doubt we will have a lot of enums.
    /// I need to implement the system in a way which will allow us to change internal structure of the game to fix this issue, if it will be an issue.
    /// </remarks>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumParameter<TEnum> : Parameter where TEnum : Enum
    {
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                 Delegates
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public delegate void ValueChangeHandler(TEnum old, TEnum current);
        public delegate void ModifiedStateChangeHandler(bool modified);

        protected delegate string SerializationHandler(TEnum value);
        protected delegate TEnum DeserializationHandler(string value);




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
        public bool IsDirty => !EqualityComparer<TEnum>.Default.Equals(m_LastValue, m_Value);
        public bool IsModified => !EqualityComparer<TEnum>.Default.Equals(m_Value, m_DefaultValue);

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
        public TEnum DefaultValue
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
        public TEnum Value
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
        protected TEnum m_DefaultValue;
        protected TEnum m_Value;

        // Local Fields:
        protected readonly SerializationHandler m_SerializationHandler;
        protected readonly DeserializationHandler m_DeserializationHandler;
        protected TEnum m_LastValue;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public EnumParameter(FullName name) : this(name, default!) { }
        public EnumParameter(FullName name, TEnum def) : base(name)
        {
            // TODO: Actually load-in the values XD
            m_Value = m_LastValue = m_DefaultValue = def;
            EngineService<ConfigurationService>.Instance.OnAfterApplyChanges += ApplyChanges;
            EngineService<ConfigurationService>.Instance.OnAfterRevertChanges += RevertChanges;

            Type type = Enum.GetUnderlyingType(typeof(TEnum));
            m_SerializationHandler = type switch
            {
                Type t when t == typeof(byte) => (v) => Base64.Byte(Convert.ToByte(v)),
                Type t when t == typeof(sbyte) => (v) => Base64.SByte(Convert.ToSByte(v)),

                Type t when t == typeof(short) => (v) => Base64.Short(Convert.ToInt16(v)),
                Type t when t == typeof(ushort) => (v) => Base64.UShort(Convert.ToUInt16(v)),

                Type t when t == typeof(int) => (v) => Base64.Int(Convert.ToInt32(v)),
                Type t when t == typeof(uint) => (v) => Base64.UInt(Convert.ToUInt32(v)),

                Type t when t == typeof(long) => (v) => Base64.Long(Convert.ToInt64(v)),
                Type t when t == typeof(ulong) => (v) => Base64.ULong(Convert.ToUInt64(v)),

                _ => (v) => throw new NotImplementedException($"Cannot serialize provided enum type '{typeof(TEnum).Name}'"),
            };

            m_DeserializationHandler = type switch
            {
                Type t when t == typeof(byte) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToByte(str)),
                Type t when t == typeof(sbyte) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToSByte(str)),

                Type t when t == typeof(short) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToShort(str)),
                Type t when t == typeof(ushort) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToUShort(str)),

                Type t when t == typeof(int) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToInt(str)),
                Type t when t == typeof(uint) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToUInt(str)),

                Type t when t == typeof(long) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToLong(str)),
                Type t when t == typeof(ulong) => (str) => (TEnum)Enum.ToObject(typeof(TEnum), Base64.ToULong(str)),

                _ => (str) => throw new NotImplementedException($"Cannot deserialize '{str}' provided enum type '{typeof(TEnum).Name}'"),
            };
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override string Serialize() => m_SerializationHandler(Value);
        public override void Deserialize(string raw) => Value = m_DeserializationHandler(raw);
        public override void ApplyChanges()
        {
            if (IsDirty) ApplyChangesForceFireCallbacks();
        }

        public override void ApplyChangesForceFireCallbacks()
        {
            TEnum old = m_LastValue;
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
            TEnum old = m_Value;
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
        public virtual void Set(TEnum value)
        {
            if (!EqualityComparer<TEnum>.Default.Equals(m_Value, value))
            {
                bool modified = IsModified;
                TEnum old = m_Value;
                m_Value = value;
                EngineService<ConfigurationService>.Instance.IsDirty |= true;
                OnValueChanged?.Invoke(old, value);
                if (IsModified != modified) OnModifiedChanged?.Invoke(!modified);
            }
        }

        private void SetDefault(TEnum value)
        {
            if (!EqualityComparer<TEnum>.Default.Equals(m_DefaultValue, value))
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
