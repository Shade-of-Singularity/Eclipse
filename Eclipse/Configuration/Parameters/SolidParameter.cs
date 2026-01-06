using Eclipse.Structs;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Configuration.Parameters
{
    /// <summary>
    /// Solid parameters can only be changed with Engine reload.
    /// You can prompt user to reload the game manually by calling (TODO) method.
    /// </summary>
    /// <remarks>
    /// Rule of thumbs as to when to use those parameters:
    /// <para>- if parameter is modifiable at runtime - use <see cref="Parameter{TValue}"/>.</para>
    /// <para>- if parameter can be modified by other mods at runtime - use <see cref="Parameter{TValue}"/>.</para>
    /// <para>- if parameter is only set at launch or engine restart - use C# Properties.</para>
    /// <para>- if parameter can be 'set' at runtime, but value will only update after reload - use <see cref="SolidParameter{TValue}"/>.</para>
    /// </remarks>
    public sealed class SolidParameter<TValue> : AbstractParameter where TValue : IConvertible, IEquatable<TValue>
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
                    value(m_StoredValue, m_StoredValue);
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
        public bool IsDirty => !EqualityComparer<TValue>.Default.Equals(m_StoredValue, m_Value);
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
        /// Internal value that will be applied to the parameter on the next <see cref="Engine"/> reload.
        /// </summary>
        public TValue StoredValue
        {
            get => m_StoredValue;
            set => Set(value);
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
        private TValue m_DefaultValue;
        private TValue m_Value;

        // Local Fields:
        private TValue m_StoredValue;




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                                Constructors
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public SolidParameter(FullName name) : this(name, default!) { }
        public SolidParameter(FullName name, TValue def) : base(name)
        {
            // TODO: Actually load-in the values from storage XD
            m_Value = m_StoredValue = m_DefaultValue = def;
            ConfigurationService.OnAfterApplyChanges += ApplyChanges;
            ConfigurationService.OnAfterRevertChanges += RevertChanges;
        }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Implementations
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        public override object GetValue() => m_Value;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string Serialize() => JsonUtility.ToJson(m_StoredValue);

        /// <summary>
        /// Deserializes <paramref name="raw"/> data and stores value in internal variable.
        /// </summary>
        /// <remarks>
        /// Actually works even after Engine initialization! However, does not fire any callbacks.
        /// </remarks>
        /// <param name="raw"></param>
        public override void Deserialize(string raw) => Value = JsonUtility.FromJson<TValue>(raw);

        /// <summary>
        /// Normally applies changed <see cref="Value"/>, but does nothing with <see cref="SolidParameter{TValue}"/>.
        /// </summary>
        public override void ApplyChanges() { }

        /// <summary>
        /// Normally applies changed <see cref="Value"/> and fides on change callbacks, but does nothing with <see cref="SolidParameter{TValue}"/>.
        /// </summary>
        public override void ApplyChangesForceFireCallbacks() { }

        /// <summary>
        /// Normally reverts changed <see cref="Value"/>, but does nothing with <see cref="SolidParameter{TValue}"/>.
        /// </summary>
        public override void RevertChanges() { }

        /// <summary>
        /// Normally reverts changed <see cref="Value"/> and fides on change callbacks, but does nothing with <see cref="SolidParameter{TValue}"/>.
        /// </summary>
        public override void RevertChangesForceFireCallbacks() { }




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Public Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        /// <summary>
        /// Resets <see cref="Value"/> to a <see cref="DefaultValue"/>.
        /// </summary>
        /// <remarks>
        /// Will only apply new value on the next <see cref="Engine"/> reload.
        /// </remarks>
        public void Reset() => Set(DefaultValue);




        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===<![CDATA[
        /// .
        /// .                                               Private Methods
        /// .
        /// ===     ===     ===     ===    ===  == =  -                        -  = ==  ===    ===     ===     ===     ===]]>
        private void Set(TValue value)
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
    }
}
